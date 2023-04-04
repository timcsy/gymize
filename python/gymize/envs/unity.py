import gymnasium as gym
import numpy as np
import time

from gymize import space
from gymize.channel import Channel, Content
from gymize.lanch import open_unity
from gymize.proto.space_pb2 import Data

class UnityEnv(gym.Env):
    metadata = { 'render_modes': [ 'rgb_array' ] }

    def __init__(self, name, file_name: str=None, observation_space=None, action_space=None, update_seconds=0.001, render_mode=None, render_fps=4):
        self.observation_space = observation_space
        self.action_space = action_space

        self.update_seconds = update_seconds
        
        self.channel = Channel(name=name)
        self.channel.connect_sync()
        time.sleep(0.5)
        open_unity(file_name)

        assert render_mode is None or render_mode in self.metadata['render_modes']
        self.render_mode = render_mode
        self.render_fps = render_fps

    def _get_obs(self, content: Content=None):
        if content is None:
            return self.observation_space.sample()
        data = Data()
        data.ParseFromString(content.raw)
        observation = {}
        for key in data.dict:
            image = data.dict[key].image
            img = space.image_to_box(image)
            observation[key] = img
        return observation

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
        self.channel.tell_sync(id="agent", content=str(action))

        content, done = self.channel.wait_message(id="agent", polling_secs=self.update_seconds)

        terminated = not done
        reward = 1 if terminated else 0  # Binary sparse rewards
        observation = self._get_obs(content)
        info = self._get_info()

        return observation, reward, terminated, False, info

    def render(self):
        if self.render_mode == 'rgb_array':
            return self._render_frame()

    def _render_frame(self):
        if self.render_mode == 'rgb_array':
            return np.transpose()

    def close(self):
        self.channel.close_sync()