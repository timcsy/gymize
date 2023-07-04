from typing import Dict, List

from pettingzoo import ParallelEnv
from gymnasium.spaces import Space
from gymnasium.utils import seeding

from gymize.bridge import Bridge, BridgeChannel

class UnityParallelEnv(ParallelEnv):
    metadata = { 'render_modes': [ 'rgb_array', 'rgb_array_list', 'video' ] }

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

    def reset(self, seed=None, options=None):
        '''
        Reset needs to initialize the `agents` attribute and must set up the
        environment so that render(), and step() can be called without issues.
        Returns the observations for each agent
        '''
        # Initialize the RNG if the seed is manually passed
        if seed is not None:
            self._np_random, seed = seeding.np_random(seed)
        
        self.bridge.reset_env() # include rewards, terminations, truncations
        self.agents = self.possible_agents[:]

        observations, _, _, _, infos = self.bridge.wait_gymize_message(wait_agents=self.agents)

        return observations, infos
        

    def step(self, actions):
        '''
        step(action) takes in an action for each agent and should return the
        - observations
        - rewards
        - terminations
        - truncations
        - infos
        dicts where each dict looks like {agent_1: item_1, agent_2: item_2}
        '''
        for agent in list(self.agents):
            if self.bridge.terminations[agent] or self.bridge.truncations[agent]:
                self.agents.remove(agent)
        
        self.bridge.set_actions(actions=actions)

        self.bridge.wait_gymize_message(wait_agents=self.agents)

        observations = self.bridge.get_observations(self.agents)
        rewards = self.bridge.get_rewards(self.agents)
        terminations = self.bridge.get_terminations(self.agents)
        truncations = self.bridge.get_truncations(self.agents)
        infos = self.bridge.get_infos(self.agents)

        return observations, rewards, terminations, truncations, infos

    def send_info(self, info, agent: str=''):
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