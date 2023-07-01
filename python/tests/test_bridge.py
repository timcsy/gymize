import os
import sys
from gymize.bridge import Bridge
from gymize import space
from gymnasium import spaces
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
    observation_spaces = { 'agent1': observation_space, 'agent2': observation_space }
    action_space = spaces.Discrete(4)
    action_spaces = { 'agent1': action_space, 'agent2': action_space }
    reward_range=(-np.inf, np.inf)
    reward_ranges = { 'agent1': reward_range, 'agent2': reward_range }
    update_seconds=0.001

    agents = [ 'agent1', 'agent2' ]

    bridge = Bridge(
        env_name='kart',
        file_name=file_name,
        action_spaces=action_spaces,
        observation_spaces=observation_spaces,
        reward_ranges=reward_ranges,
        agents=agents,
        update_seconds=update_seconds
    )

    bridge.reset_env()
    observations, _, _, _, infos = bridge.wait_gymize_message()

    for i in range(2):
        actions = { agent_name: action_space.sample() for agent_name in agents }
        print(actions)
        print(f'i = {i}')

        bridge.set_actions(actions)
        bridge.wait_gymize_message()

        observations = bridge.get_observations(agents)
        rewards = bridge.get_rewards(agents)
        terminateds = bridge.get_terminations(agents)
        truncateds = bridge.get_truncations(agents)
        infos = bridge.get_infos(agents)

        print(observations)
        print(rewards)
        print(terminateds)
        print(truncateds)
        print(infos)

        space.save_image(observations['agent1']['CameraFront'], f'imgs/CameraFront_{i}.jpg')
        bridge.send_info('agent1', 'test123')
        bridge.send_info('agent2', 'test456')

        end = True
        for agent_name in agents:
            if terminateds[agent_name] or truncateds[agent_name]:
                pass
            else:
                end = False
        if end:
            bridge.reset_env()
            observations, _, _, _, infos = bridge.wait_gymize_message()
    
    bridge.close()