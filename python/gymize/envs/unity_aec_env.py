from typing import List
import gymnasium as gym
import numpy as np
import time

from gymize import space
from gymize.channel import Channel, Content
from gymize.lanch import launch_env
from gymize.proto.gymize_pb2 import GymizeProto, InfoProto, ObservationProto, RewardProto

class UnityAECEnv(gym.Env):
    metadata = { 'render_modes': [ 'rgb_array' ] }

    def __init__(self, env_name, file_name: str=None, observation_space=None, action_space=None, agent_selection: str='agent', update_seconds=0.001, render_mode=None, render_fps=4):
        self.observation_space = observation_space
        self.action_space = action_space

        self.agent_selection = agent_selection
        self.terminations = { self.agent_selection: False }
        self.truncations = { self.agent_selection: False }

        self.update_seconds = update_seconds
        
        self.channel = Channel(name=env_name)
        self.channel.connect_sync()
        time.sleep(0.5)
        launch_env(file_name)

        assert render_mode is None or render_mode in self.metadata['render_modes']
        self.render_mode = render_mode
        self.render_fps = render_fps

    def _get_obs(self, observation_protos: List[ObservationProto]=None):
        observations = { self.agent_selection: self.observation_space.sample() }
        if observation_protos is None:
            return observations
        observations = space.tuple_to_list(observations)
        renew_list = set()
        for obs in observation_protos:
            observations = space.merge(observations, obs.locator, obs.observation, renew_list)
        return space.list_to_tuple(observations)
    
    def _get_reward(self, reward_protos: List[RewardProto]=None):
        rewards = { self.agent_selection: 0 }
        if reward_protos is None:
            return rewards
        for reward_proto in reward_protos:
            rewards[reward_proto.agent] = reward_proto.reward
        return rewards
    
    def _get_terminations(self, terminated_agents: List[str]=None):
        if terminated_agents is None:
            return self.terminations
        for agent in terminated_agents:
            if agent == '':
                self.terminations = { agent: True for agent in self.terminations.keys() }
                break
            else:
                self.terminations[agent] = True
        return self.terminations
    
    def _get_truncations(self, truncated_agents: List[str]=None):
        if truncated_agents is None:
            return self.truncations
        for agent in truncated_agents:
            if agent == '':
                self.truncations = { agent: True for agent in self.truncations.keys() }
                break
            else:
                self.truncations[agent] = True
        return self.truncations

    def _get_infos(self, info_protos: List[InfoProto]=None):
        infos = { self.agent_selection: { 'env': [], 'agent': [] } }
        if info_protos is None:
            return infos
        env_info_proto = None
        for info_proto in info_protos:
            if info_proto.agent != '':
                infos[info_proto.agent] = { 'agent': [] }
                for instance_proto in info_proto.infos:
                    infos[info_proto.agent]['agent'].append(space.from_proto(instance_proto))
            else:
                env_info_proto = info_proto
        if env_info_proto is not None:
            env_infos = []
            for instance_proto in env_info_proto.infos:
                env_infos.append(space.from_proto(instance_proto))
            for agent in infos.keys():
                infos[agent]['env'] = env_infos

        return infos
    
    def parse_message(self, content: Content=None):
        if content is None:
            return self._get_obs(None), self._get_reward(None), self._get_terminations(None), self._get_truncations(None), self._get_infos(None)
        gymize_proto = GymizeProto()
        gymize_proto.ParseFromString(content.raw)
        observations = self._get_obs(gymize_proto.observations)
        rewards = self._get_reward(gymize_proto.rewards)
        terminations = self._get_terminations(gymize_proto.terminated_agents)
        truncations = self._get_truncations(gymize_proto.truncated_agents)
        infos = self._get_infos(gymize_proto.infos)
        return observations, rewards, terminations, truncations, infos

    def reset(self, seed=None, options=None):
        # We need the following line to seed self.np_random
        super().reset(seed=seed)

        gymize_proto = GymizeProto()
        gymize_proto.reset_agents.extend([''] + [self.agent_selection])

        msg = gymize_proto.SerializeToString()

        self.channel.tell_sync(id='_gym_', content=msg)

        observations = self._get_obs()
        infos = self._get_infos()

        return observations[self.agent_selection], infos[self.agent_selection]

    def step(self, action):
        # send action
        self.channel.tell_sync(id='_gym_', content=space.to_gymize(action, self.agent_selection))

        content, done = self.channel.wait_message(id='_gym_', polling_secs=self.update_seconds)

        observations, rewards, terminations, truncations, infos = self.parse_message(content)
        truncated = done or truncations[self.agent_selection]

        return observations[self.agent_selection], rewards[self.agent_selection], terminations[self.agent_selection], truncated, infos[self.agent_selection]

    def render(self):
        if self.render_mode == 'rgb_array':
            return self._render_frame()

    def _render_frame(self):
        if self.render_mode == 'rgb_array':
            return np.transpose()

    def send_info(self, info):
        gymize_proto = GymizeProto()
        info_proto = InfoProto(
            agent=self.agent_selection,
            infos=[space.to_proto(info)]
        )
        gymize_proto.infos.append(info_proto)

        msg = gymize_proto.SerializeToString()

        self.channel.tell_sync(id='_gym_', content=msg)

    def close(self):
        self.channel.close_sync()