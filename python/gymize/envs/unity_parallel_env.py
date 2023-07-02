from typing import List

from pettingzoo import ParallelEnv
from gymnasium.spaces import Space
from gymnasium.utils import seeding

from gymize.bridge import Bridge

class UnityParallelEnv(ParallelEnv):
    metadata = { 'render_modes': [ 'rgb_array' ] }

    def __init__(self, env_name, file_name: str=None, action_spaces=None, observation_spaces=None, agent_names: List[str]={}, update_seconds=0.001, render_mode=None, views: List[str]=[''], render_fps=4):
        self.bridge = Bridge(
            env_name=env_name,
            file_name=file_name,
            action_spaces=action_spaces,
            observation_spaces=observation_spaces,
            agents=agent_names,
            update_seconds=update_seconds
        )
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
        self.bridge.set_actions(actions=actions)

        observations, rewards, terminations, truncations, infos = self.bridge.wait_gymize_message(wait_agents=self.agents)

        for agent in list(self.agents):
            if terminations[agent] or truncations[agent]:
                self.agents.remove(agent)

        observations = self.bridge.get_observations(self.agents)
        rewards = self.bridge.get_rewards(self.agents)
        terminations = self.bridge.get_terminations(self.agents)
        truncations = self.bridge.get_truncations(self.agents)
        infos = self.bridge.get_infos(self.agents)

        return observations, rewards, terminations, truncations, infos

    def send_info(self, info, agent: str=''):
        self.bridge.send_info(agent=agent, info=info)
    
    def render(self):
        if self.render_mode == 'rgb_array':
            return self.bridge.get_frame(self.views)
        elif self.render_mode == 'rgb_array_list':
            return self.bridge.get_frames(self.views)
        elif self.render_mode == 'video':
            return self.bridge.get_recording(self.views)

    def close(self):
        self.bridge.close()