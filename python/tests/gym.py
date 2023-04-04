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
            "CameraFront": spaces.Box(0, 255, shape=(1080, 1920, 3), dtype=np.uint8)
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
            space.save_image(img, f'imgs/{key}_{i}.png')

        if terminated or truncated:
            observation, info = env.reset()

    env.close()


# 這是一個可以動的，至少可以輸出好幾張圖，測試 VirtualGL
# TODO: NetSharp、參考 Gym.Net (Box) 實作，甚至直接像 Gym 那樣是直接用 ndarray 當基本 Data
# TODO: inherit a class from NDArray, which implements constructors and static method of NDArray
# TODO: 定義 gym、PettingZoo（Parallel、AEC）的傳輸格式，要考量到時候的玩家 Script 和 Wrapper，infos 之類的是對於每個 Agent 個別處理，可以寫一個 listener 來分類 observation 和 infos
# TODO: Gym 的 Environment 定義和 PettingZoo（Parallel、AEC）的要分開，PettingZoo 只用一個 channel 但不同 id，Gym 可用多個 channel 但只有一個 id
# TODO: 處理 reset
# TODO: 在哪裡啟動 Unity（會有多人共用一個遊戲（PettingZoo）、平行加速開多個遊戲（Gym Vector），還有不同玩家的 Wrapper）
# TODO: 了解 gym 如何開啟多環境，可以參考 ML-Agents: https://github.com/Unity-Technologies/ml-agents/tree/main/ml-agents-envs/mlagents_envs/envs
# TODO: Parallel、AEC
# TODO: VirtualGL