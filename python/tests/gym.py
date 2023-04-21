import os
import sys

import gymnasium as gym
from gymnasium import spaces

from gymize import space
import numpy as np

if __name__ == '__main__':
    file_name = None
    if len(sys.argv) > 1:
        file_name = sys.argv[1]
    
    if not os.path.exists('imgs'):
        os.makedirs('imgs')

    observation_space = spaces.Dict(
        {
            "CameraFront": spaces.Box(0, 255, shape=(540, 960, 3), dtype=np.uint8)
        }
    )
    action_space = spaces.Discrete(4)

    env = gym.make('gymize/Unity-v0', name='kart', file_name=file_name, observation_space=observation_space, action_space=action_space)
    observation, info = env.reset()

    for i in range(100):
        action = env.action_space.sample()  # agent policy that uses the observation and info
        print(f'i = {i}')
        observation, reward, terminated, truncated, info = env.step(action)
        for key, img in observation.items():
            space.save_image(img, f'imgs/{key}_{i}.jpg')

        if terminated or truncated:
            observation, info = env.reset()

    env.close()