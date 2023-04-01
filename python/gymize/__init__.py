from gymnasium.envs.registration import register

register(
    id='gymize/Unity-v0',
    entry_point='gymize.envs:UnityEnv'
)