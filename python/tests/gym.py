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


# TODO: NetSharp、參考 Gym.Net Box 實作，甚至直接像 Gym 那樣是直接用 ndarray 當基本 Data
# TODO: 定義 gym、PettingZoo 的傳輸格式
# TODO: 處理 reset
# TODO: 在哪裡啟動 Unity（會有多人共用一個遊戲（PettingZoo）、平行加速開多個遊戲（Gym Vector），還有不同玩家的 Wrapper）
# TODO: 了解 gym 如何開啟多環境，可以參考 ML-Agents: https://github.com/Unity-Technologies/ml-agents/tree/main/ml-agents-envs/mlagents_envs/envs
# TODO: Parallel、AEC
# TODO: VirtualGL