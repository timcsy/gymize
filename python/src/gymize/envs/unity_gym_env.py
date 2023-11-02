from typing import Dict, List

import gymnasium as gym

from gymize.bridge import Bridge, BridgeChannel

class UnityGymEnv(gym.Env, BridgeChannel):
    metadata = { 'render_modes': [ 'rgb_array', 'rgb_array_list', 'video' ], 'render_fps': 4 }

    def __init__(self, env_name, file_name: str=None, action_space=None, observation_space=None, reward_range=(-float('inf'), float('inf')), agent_name: str='agent', update_seconds=0.001, render_mode=None, views: List[str]=[''], render_fps=4):
        self.bridge = Bridge(
            env_name=env_name,
            file_name=file_name,
            action_spaces={ agent_name: action_space },
            observation_spaces={ agent_name: observation_space },
            reward_ranges={ agent_name: reward_range },
            agents=[ agent_name ],
            render_mode=render_mode,
            update_seconds=update_seconds
        )
        BridgeChannel.__init__(self, self.bridge)

        self.action_space = action_space
        self.observation_space = observation_space
        self.reward_range = reward_range
        self.agent = agent_name
        self.views = views # for rendering

        assert render_mode is None or render_mode in self.metadata['render_modes']
        self.render_mode = render_mode
        self.render_fps = render_fps

    def reset(self, seed=None, options=None):
        # We need the following line to seed self.np_random
        super().reset(seed=seed, options=options)

        self.bridge.reset_env()
        observations, _, _, _, infos = self.bridge.wait_gymize_message(wait_agents=[ self.agent ])

        return observations[self.agent], infos[self.agent]

    def step(self, action):
        self.bridge.set_actions({ self.agent: action })

        self.bridge.wait_gymize_message(wait_agents=[ self.agent ])

        observation = self.bridge.get_observations([ self.agent ])[self.agent]
        reward = self.bridge.get_rewards([ self.agent ])[self.agent]
        terminated = self.bridge.get_terminations([ self.agent ])[self.agent]
        truncated = self.bridge.get_truncations([ self.agent ])[self.agent]
        info = self.bridge.get_infos([ self.agent ])[self.agent]

        return observation, reward, terminated, truncated, info

    def send_info(self, info, agent: str=None):
        if agent is None:
            agent = self.agent
        self.bridge.send_info(info=info, agent=agent)

    def begin_render(self, screen_width: int=-1, screen_height: int=-1, fullscreen: bool=False):
        config = {
            'screen_width': screen_width,
            'screen_height': screen_height,
            'fullscreen': fullscreen
        }
        if self.render_mode == 'rgb_array':
            config['is_single_frame'] = True
            self.bridge.begin_render({ name: config for name in self.views })
        elif self.render_mode == 'rgb_array_list' or self.render_mode == 'video':
            config['is_single_frame'] = False
            self.bridge.begin_render({ name: config for name in self.views })
    
    def end_render(self):
        self.bridge.end_render(self.views)
    
    def render_all(self, video_paths: Dict[str, str]={}):
        return self.bridge.render_all(names=self.views, video_paths=video_paths)
    
    def render(self):
        name = None if self.views is None or len(self.views) == 0 else self.views[0]
        return self.bridge.render(name=name)

    def close(self):
        self.bridge.close()