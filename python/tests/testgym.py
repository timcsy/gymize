import gymnasium as gym

import gymize

env = gym.make('gymize/Unity-v0', name='kart')
observation, info = env.reset()

for _ in range(2):
    action = env.action_space.sample()  # agent policy that uses the observation and info
    observation, reward, terminated, truncated, info = env.step(action)
    print(observation)

    if terminated or truncated:
        observation, info = env.reset()

env.close()


# TODO: 在哪裡啟動 Unity, VirtualGL
# TODO: 了解 gym 如何開啟多環境，可以參考 ML-Agents: https://github.com/Unity-Technologies/ml-agents/tree/main/ml-agents-envs/mlagents_envs/envs