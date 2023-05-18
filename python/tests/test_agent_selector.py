'''
$ python test_agent_selector.py 
Connected to Signal Server: ws://localhost:50864/
Please start the Unity game in the Unity Editor or open the game manually!
/.../gymnasium/utils/passive_env_checker.py:35: UserWarning: WARN: A Box observation space has an unconventional shape (neither an image, nor a 1D vector). We recommend flattening the observation to have only a 1D vector or use a custom policy to properly process the data. Actual observation shape: (2, 3)
  logger.warn(
/.../gymnasium/utils/passive_env_checker.py:35: UserWarning: WARN: A Box observation space has an unconventional shape (neither an image, nor a 1D vector). We recommend flattening the observation to have only a 1D vector or use a custom policy to properly process the data. Actual observation shape: (4, 8)
  logger.warn(
2
i = 0
Start Peer Server: ws://localhost:57195
{'A': 1, 'B': array([[2, 3, 4],
       [7, 8, 9]], dtype=uint8), 'C': (2, 'PAIA'), 'D': (46, 4, 6, 8), 'E': array([[154, 118,  71, 200, 237,  47,  16, 248],
       [117, 174, 236,   1, 125,   2, 130,   3],
       [222, 189, 206,   5, 192,   6,  26,   7],
       [ 36,  23, 223,   9,  32,  10, 148,  11]], dtype=uint8), 'F': (array([[1, 2, 3],
       [4, 5, 6],
       [7, 8, 9]], dtype=int32), array([[11, 12, 13],
       [14, 15, 16],
       [17, 18, 19]], dtype=int32), array([[21, 22, 23],
       [24, 25, 26],
       [27, 28, 29]], dtype=int32))}
The Peer connection: ws://localhost:57195 is closed
The Signal Server connection: ws://localhost:50864/ is closed
'''

import numpy as np
import os
import sys

import gymnasium as gym
from gymnasium import spaces

import gymize

if __name__ == '__main__':
    file_name = None
    if len(sys.argv) > 1:
        file_name = sys.argv[1]
    
    if not os.path.exists('imgs'):
        os.makedirs('imgs')

    # observation_space = spaces.Discrete(1000)
    observation_space = spaces.Dict(
        {
            'A': spaces.Discrete(100),
            'B': spaces.Box(0, 255, shape=(2, 3), dtype=np.uint8),
            'C': spaces.Tuple(
                [
                    spaces.Discrete(100),
                    spaces.Text(100)
                ]
            ),
            'D': spaces.Tuple([ spaces.Discrete(100) ] * 4),
            'E': spaces.Box(0, 255, shape=(4, 8), dtype=np.uint8),
            # 'F': spaces.Sequence(spaces.Box(0, 255, shape=(3, 3), dtype=np.int32)),
            'F': spaces.Sequence(spaces.Box(0, 255, shape=(3, 3), dtype=np.int32), stack=True),
        }
    )

    action_space = spaces.Discrete(4)

    env = gym.make('gymize/Unity-v0', env_name='kart', file_name=file_name, observation_space=observation_space, action_space=action_space)
    observation, info = env.reset()

    for i in range(1):
        action = env.action_space.sample()  # agent policy that uses the observation and info
        print(action)
        print(f'i = {i}')
        observation, reward, terminated, truncated, info = env.step(action)
        print(observation)

        if terminated or truncated:
            observation, info = env.reset()

    env.close()