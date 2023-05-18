import numpy as np
import os
import sys

import gymnasium as gym
from gymnasium import spaces

import gymize
from gymize import space

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

    env = gym.make('gymize/Unity-v0', env_name='kart', file_name=file_name, observation_space=observation_space, action_space=action_space)
    observation, info = env.reset()

    for i in range(2):
        action = env.action_space.sample()  # agent policy that uses the observation and info
        print(action)
        print(f'i = {i}')
        observation, reward, terminated, truncated, info = env.step(action)
        print(observation)
        print(reward)
        print(terminated)
        print(truncated)
        print(info)
        space.save_image(observation['CameraFront'], f'imgs/CameraFront_{i}.jpg')
        env.send_info('test123')

        if terminated or truncated:
            observation, info = env.reset()

    env.close()