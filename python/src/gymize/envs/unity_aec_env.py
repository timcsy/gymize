from typing import Dict, List

from pettingzoo import AECEnv
from pettingzoo.utils import agent_selector
from gymnasium.spaces import Space
from gymnasium.utils import seeding

from gymize.bridge import Bridge, BridgeChannel

class UnityAECEnv(AECEnv):
    metadata = { 'render_modes': [ 'rgb_array', 'rgb_array_list', 'video' ], 'render_fps': 4 }

    def __init__(self, env_name, file_name: str=None, action_spaces=None, observation_spaces=None, agent_names: List[str]={}, update_seconds=0.001, render_mode=None, views: List[str]=[''], render_fps=4):
        self.bridge = Bridge(
            env_name=env_name,
            file_name=file_name,
            action_spaces=action_spaces,
            observation_spaces=observation_spaces,
            agents=agent_names,
            render_mode=render_mode,
            update_seconds=update_seconds
        )
        BridgeChannel.__init__(self, self.bridge)

        self.possible_agents = agent_names
        self.agents = agent_names[:]
        self.action_spaces = action_spaces
        self.observation_spaces = observation_spaces

        self.views = views # for rendering

        assert render_mode is None or render_mode in self.metadata['render_modes']
        self.render_mode = render_mode
        self.render_fps = render_fps
    
    def observation_space(self, agent: str) -> Space:
        '''
        Takes in agent and returns the observation space for that agent.
        MUST return the same value for the same agent name
        Default implementation is to return the observation_spaces dict
        '''
        return self.observation_spaces[agent]

    def action_space(self, agent: str) -> Space:
        '''
        Takes in agent and returns the action space for that agent.
        MUST return the same value for the same agent name
        Default implementation is to return the action_spaces dict
        '''
        return self.action_spaces[agent]
    
    @property
    def terminations(self):
        return self.bridge.terminations
    
    @property
    def truncations(self):
        return self.bridge.truncations
    
    @property
    def rewards(self):
        return self.bridge.rewards
    
    @property
    def infos(self):
        return self.bridge.infos

    def reset(self, seed=None, options=None):
        '''
        Reset needs to initialize the following attributes
        - agents
        - rewards
        - _cumulative_rewards
        - terminations
        - truncations
        - infos
        - agent_selection
        And must set up the environment so that render(), step(), and observe()
        can be called without issues.
        Here it sets up the state dictionary which is used by step() and the observations dictionary which is used by step() and observe()
        '''
        # Initialize the RNG if the seed is manually passed
        if seed is not None:
            self._np_random, seed = seeding.np_random(seed)
        
        self.bridge.reset_env() # include rewards, terminations, truncations
        self.agents = self.possible_agents[:]
        self._cumulative_rewards = { agent: 0 for agent in self.agents }

        '''
        Our agent_selector utility allows easy cyclic stepping through the agents list.
        '''
        self._agent_selector = agent_selector(self.agents)
        self.agent_selection = self._agent_selector.next()

    def step(self, action):
        '''
        step(action) takes in an action for the current agent (specified by
        agent_selection) and needs to update
        - rewards
        - _cumulative_rewards (accumulating the rewards)
        - terminations
        - truncations
        - infos
        - agent_selection (to the next agent)
        And any internal state used by observe() or render()
        '''
        if (
            self.terminations[self.agent_selection]
            or self.truncations[self.agent_selection]
        ):
            # handles stepping an agent which is already dead
            # accepts a None action for the one agent, and moves the agent_selection to
            # the next dead agent,  or if there are no more dead agents, to the next live agent
            self._was_dead_step(action)
            return

        agent = self.agent_selection

        # the agent which stepped last had its _cumulative_rewards accounted for
        # (because it was returned by last()), so the _cumulative_rewards for this
        # agent should start again at 0
        self._cumulative_rewards[agent] = 0

        self.bridge.set_actions({ agent: action })

        # selects the next agent.
        self.agent_selection = self._agent_selector.next()
        # Adds .rewards to ._cumulative_rewards
        self._accumulate_rewards()

    def observe(self, agent):
        '''
        Observe should return the observation of the specified agent. This function
        should return a sane observation (though not necessarily the most up to date possible)
        at any time after reset() is called.
        '''
        # we wait for Unity side here
        self.bridge.wait_gymize_message(wait_agents=[ agent ])
        return self.bridge.get_observations([ agent ])[agent]

    def send_info(self, info, agent: str=None):
        if agent is None:
            agent = self.agent_selection
        self.bridge.send_info(agent=agent, info=info)
    
    def begin_render(self, screen_width: int=-1, screen_height: int=-1, fullscreen: bool=False):
        config = {
            'screen_width': screen_width,
            'screen_height': screen_height,
            'fullscreen': fullscreen
        }
        if self.render_mode == 'rgb_array':
            config['is_single_frame'] = True
            self.bridge.begin_render({ name: config for name in self.views })
        elif self.render_mode == 'rgb_array_list' or self.render_mode == 'video':
            config['is_single_frame'] = False
            self.bridge.begin_render({ name: config for name in self.views })
    
    def end_render(self):
        self.bridge.end_render(self.views)
    
    def render_all(self, video_paths: Dict[str, str]={}):
        return self.bridge.render_all(names=self.views, video_paths=video_paths)
    
    def render(self):
        name = None if self.views is None or len(self.views) == 0 else self.views[0]
        return self.bridge.render(name=name)

    def close(self):
        self.bridge.close()