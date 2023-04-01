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