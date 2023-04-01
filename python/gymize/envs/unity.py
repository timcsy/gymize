import time
import gymnasium as gym
from gymnasium import spaces

import numpy as np

from gymize.channel import Channel
from gymize.lanch import open_unity

class UnityEnv(gym.Env):
    metadata = { 'render_modes': [ 'rgb_array' ] }

    def __init__(self, name, file_name: str=None, observation_space=None, action_space=None, update_seconds=0.001, render_mode=None, render_fps=4):
        self.channel = Channel(name=name)
        self.channel.connect_sync()
        time.sleep(0.5)
        open_unity(file_name)
        self.observation_space = spaces.Dict(
            {
                "Camera": spaces.Box(0, 255, shape=(1,), dtype=int)
            }
        )
        self.action_space = spaces.Discrete(4)

        self.update_seconds = update_seconds

        assert render_mode is None or render_mode in self.metadata['render_modes']
        self.render_mode = render_mode
        self.render_fps = render_fps

    def _get_obs(self):
        return self.observation_space.sample()

    def _get_info(self):
        return {}

    def reset(self, seed=None, options=None):
        # We need the following line to seed self.np_random
        super().reset(seed=seed)

        observation = self._get_obs()
        info = self._get_info()

        return observation, info

    def step(self, action):
        # send action
        result = self.channel.tell(action)
        terminated = False
        reward = 1 if terminated else 0  # Binary sparse rewards
        observation = self._get_obs()
        info = self._get_info()

        return observation, reward, terminated, False, info

    def render(self):
        if self.render_mode == 'rgb_array':
            return self._render_frame()

    def _render_frame(self):
        if self.render_mode == 'rgb_array':
            return np.transpose()

    def close(self):
        pass