'''
Start Signal Server: ws://127.0.0.1:50864
Please start the Unity game in the Unity Editor or open the game manually!
Connected to Signal Server: ws://127.0.0.1:50864
Connection: test, is initialized with id: c6fda256-d2c2-4987-ad08-14c4cf499c74
Start Peer Server: ws://127.0.0.1:53701
Connection id: c6fda256-d2c2-4987-ad08-14c4cf499c74, is using channel: ws://127.0.0.1:53701
Agent name: agent1, action:3
Agent name: agent2, action:1
Agent name: agent3, action:0
Agent name: agent1, action:3
Agent name: agent2, action:3
Agent name: agent3, action:2
Agent name: agent1, action:2
Agent name: agent2, action:3
Agent name: agent3, action:2
Agent name: agent1, action:0
Agent name: agent2, action:3
Agent name: agent3, finish
Agent name: agent1, action:2
Agent name: agent2, finish
Agent name: agent1, finish
Connection id: c6fda256-d2c2-4987-ad08-14c4cf499c74, is closed
The Peer connection: ws://127.0.0.1:53701 is closed
The Signal Server connection: ws://127.0.0.1:50864 is closed
'''

import numpy as np
import sys

import gymnasium as gym
from gymnasium import spaces

from gymize.envs import UnityAECEnv

if __name__ == '__main__':
    file_name = None
    if len(sys.argv) > 1:
        file_name = sys.argv[1]

    agents = ['agent1', 'agent2', 'agent3']

    observation_space = spaces.Dict(
        {
            "num": spaces.Box(-2147483648, 2147483647, dtype=np.int32)
        }
    )
    observation_spaces = { agent: observation_space for agent in agents }

    action_space = spaces.Discrete(4)
    action_spaces = { agent: action_space for agent in agents }

    render_mode = None

    env = UnityAECEnv(env_name='test', file_name=file_name, agent_names=agents, observation_spaces=observation_spaces, action_spaces=action_spaces, render_mode=render_mode)

    env.reset()

    for agent in env.agent_iter():
        observation, reward, terminated, truncated, info = env.last()
        
        if terminated or truncated:
            action = None
            print('Agent name: ' + env.agent_selection + ', finish')
        else:
            action = env.action_space(env.agent_selection).sample()
            print('Agent name: ' + env.agent_selection + ', action:' + str(action))
        
        env.step(action)

    env.close()

    # for i in range(2):
    #     action = env.action_space.sample()  # agent policy that uses the observation and info
    #     print(action)
    #     print(f'i = {i}')
    #     observation, reward, terminated, truncated, info = env.step(action)
    #     print(observation)
    #     print(reward)
    #     print(terminated)
    #     print(truncated)
    #     print(info)
    #     env.send_info('test123')

    #     if terminated or truncated:
    #         observation, info = env.reset()

    # env.close()