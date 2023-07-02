from typing import List
import numpy as np

import gymnasium as gym

from gymize.bridge import Bridge

class UnityGymEnv(gym.Env):
    metadata = { 'render_modes': [ 'rgb_array' ] }

    def __init__(self, env_name, file_name: str=None, action_space=None, observation_space=None, reward_range=(-np.inf, np.inf), agent_name: str='agent', update_seconds=0.001, render_mode=None, views: List[str]=[''], render_fps=4):
        self.bridge = Bridge(
            env_name=env_name,
            file_name=file_name,
            action_spaces={ agent_name: action_space },
            observation_spaces={ agent_name: observation_space },
            reward_ranges={ agent_name: reward_range },
            agents=[ agent_name ],
            update_seconds=update_seconds
        )
        self.action_space = action_space
        self.observation_space = observation_space
        self.reward_range = reward_range
        self.agent = agent_name
        self.views = views # for rendering

        assert render_mode is None or render_mode in self.metadata['render_modes']
        self.render_mode = render_mode
        self.render_fps = render_fps

    def reset(self, seed=None, options=None):
        # We need the following line to seed self.np_random
        super().reset(seed=seed)

        self.bridge.reset_env()
        observations, _, _, _, infos = self.bridge.wait_gymize_message(wait_agents=[ self.agent ])

        return observations[self.agent], infos[self.agent]

    def step(self, action):
        self.bridge.set_actions({ self.agent: action })

        self.bridge.wait_gymize_message(wait_agents=[ self.agent ])

        observation = self.bridge.get_observations([ self.agent ])[self.agent]
        reward = self.bridge.get_rewards([ self.agent ])[self.agent]
        terminated = self.bridge.get_terminations([ self.agent ])[self.agent]
        truncated = self.bridge.get_truncations([ self.agent ])[self.agent]
        info = self.bridge.get_infos([ self.agent ])[self.agent]

        return observation, reward, terminated, truncated, info

    def send_info(self, info):
        self.bridge.send_info(self.agent, info)
    
    def render(self):
        if self.render_mode == 'rgb_array':
            return self.bridge.get_frame(self.views)
        elif self.render_mode == 'rgb_array_list':
            return self.bridge.get_frames(self.views)
        elif self.render_mode == 'video':
            return self.bridge.get_recording(self.views)

    def close(self):
        self.bridge.close()