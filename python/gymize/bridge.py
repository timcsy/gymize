import time
from typing import Any, Dict, List, Set, Tuple

from gymnasium.spaces import Space

from gymize import space
from gymize.channel import Channel, Content
from gymize.lanch import launch_env
from gymize.proto.gymize_pb2 import GymizeProto, ActionProto, ObservationProto, RewardProto, InfoProto

class Bridge:
    def __init__(
        self,
        env_name: str,
        file_name: str=None,
        action_spaces: Dict[str, Space]={},
        observation_spaces: Dict[str, Space]={},
        reward_ranges: Dict[str, Tuple[float, float]]={},
        agents: List[str]=[],
        update_seconds: float=0.001
    ):
        self.action_spaces = action_spaces
        self.observation_spaces = observation_spaces
        self.reward_ranges = reward_ranges
        self.possible_agents = agents

        self.update_seconds = update_seconds

        self.request_agents = []
        self.responsed = { agent: False for agent in self.possible_agents } # { agent: responsed }
        self.reset_requests = []
        self.actions = {}
        self.observations = { agent: self.observation_spaces[agent].sample() for agent in self.possible_agents }
        self.rewards = { agent: 0.0 for agent in self.possible_agents }
        self.terminations = { agent: False for agent in self.possible_agents }
        self.truncations = { agent: False for agent in self.possible_agents }
        self.infos = { agent: { 'env': [], 'agent': [] } for agent in self.possible_agents }
        
        self.channel = Channel(name=env_name)
        self.channel.connect_sync()
        time.sleep(0.5)
        launch_env(file_name)
    
    def add_request_agents(self, agents: List[str]) -> None:
        self.request_agents = list(set(self.request_agents + agents))
        self.send_forward_messages()

    def reset_env(self) -> None:
        self.reset_agents([''] + self.possible_agents)

    def reset_agents(self, agents: List[str]) -> None:
        # not terminate or truncate anymore, renew possible_agents
        for agent in agents:
            if agent != '':
                self.responsed[agent] = False
                self.rewards[agent] = 0
                self.terminations[agent] = False
                self.truncations[agent] = False
        
        self.reset_requests = list(set(self.reset_requests + agents))
        self.add_request_agents(agents)

    def set_actions(self, actions: Dict[str, Any], with_env: bool=True) -> None:
        # TODO v: if there are more than one agent, then wait for obsertvation? Not here, on wait_gymize_message
        self.actions = { **self.actions, **actions }
        env = []
        if with_env:
            env += ['']
        self.add_request_agents(env + list(actions.keys()))

    def get_observations(self, agents: List[str]):
        return { agent: self.observations[agent] for agent in agents }

    def get_rewards(self, agents: List[str]):
        return { agent: self.rewards[agent] for agent in agents }

    def get_terminations(self, agents: List[str]):
        return { agent: self.terminations[agent] for agent in agents }

    def get_truncations(self, agents: List[str]):
        return { agent: self.truncations[agent] for agent in agents }

    def get_infos(self, agents: List[str]):
        return { agent: self.infos[agent] for agent in agents }
    
    def send_info(self, info, agent):
        gymize_proto = GymizeProto()
        info_proto = InfoProto(agent=agent, infos=[space.to_proto(info)])
        gymize_proto.infos.append(info_proto)
        self.send_gymize_message(gymize_proto)
    
    def get_frame(self, names: List[str]):
        pass

    def get_frames(self, names: List[str]):
        pass

    def get_recording(self, names: List[str]):
        # TODO: video
        pass

    def close(self) -> None:
        self.channel.close_sync()


    def send_forward_messages(self) -> None:
        # send requests, resets, actions to another side
        gymize_proto = GymizeProto()
        gymize_proto.request_agents.extend(self.request_agents)
        gymize_proto.reset_agents.extend(self.reset_requests)
        # TODO v: only send requested actions? There is no request from Unity to Python
        for agent, action in self.actions.items():
            action_proto = ActionProto()
            action_proto.agent = agent
            action_proto.action.MergeFrom(space.to_proto(action))
            gymize_proto.actions.append(action_proto)
        self.send_gymize_message(gymize_proto)
        
        self.request_agents = []
        self.reset_requests = []
        self.actions = {}

    def send_gymize_message(self, gymize_proto: GymizeProto) -> None:
        msg = gymize_proto.SerializeToString()
        self.channel.tell_sync(id='_gym_', content=msg)
    
    def wait_gymize_message(self, wait_agents: List[str]=[]) -> None:
        # agents: the agents that have to especially wait for, blocking
        # TODO v: waiting for agents
        while True:
            content, done = self.channel.wait_message(id='_gym_', polling_secs=self.update_seconds)
            observations, rewards, terminations, truncations, infos = self.parse_message(content=content, done=done)
            if self.is_agents_collected(agents=wait_agents):
                for agent in wait_agents:
                    self.responsed[agent] = False
                break
        return observations, rewards, terminations, truncations, infos

    def is_agents_collected(self, agents: List[str]) -> bool:
        for agent in agents:
            if not self.responsed[agent]:
                return False
        return True
    
    def parse_message(self, content: Content=None, done: bool=False):
        observations = self.parse_observations(None)
        rewards = self.parse_rewards(None)
        terminations = self.parse_terminations(None)
        truncations = self.parse_truncations(None, done)
        infos = self.parse_infos(None)

        if content is not None:
            gymize_proto = GymizeProto()
            gymize_proto.ParseFromString(content.raw)

            for agent in gymize_proto.response_agents:
                self.responsed[agent] = True

            observations = self.parse_observations(gymize_proto.observations)
            rewards = self.parse_rewards(gymize_proto.rewards)
            terminations = self.parse_terminations(gymize_proto.terminated_agents)
            truncations = self.parse_truncations(gymize_proto.truncated_agents, done)
            infos = self.parse_infos(gymize_proto.infos)
        
        return observations, rewards, terminations, truncations, infos

    def parse_observations(self, observation_protos: List[ObservationProto]=None):
        if observation_protos is None:
            return self.observations

        observations = space.tuple_to_list(self.observations)
        # TODO v: when to renew list? everytime or request agent only? but parse means requested
        renew_list = set()
        for obs in observation_protos:
            observations = space.merge(observations, obs.locator, obs.observation, renew_list)
        self.observations = space.list_to_tuple(observations)
        return self.observations
    
    def parse_rewards(self, reward_protos: List[RewardProto]=None):
        if reward_protos is None:
            return self.rewards
        
        for reward_proto in reward_protos:
            if reward_proto.agent != '':
                self.rewards[reward_proto.agent] = reward_proto.reward
        return self.rewards
    
    def parse_terminations(self, terminated_agents: List[str]=None):
        if terminated_agents is None:
            return self.terminations
        
        for agent in terminated_agents:
            if agent == '':
                self.terminations = { agent: True for agent in self.terminations.keys() }
                break
            else:
                self.terminations[agent] = True
        return self.terminations
    
    def parse_truncations(self, truncated_agents: List[str]=None, done: bool=False):
        if done:
            self.truncations = { agent: True for agent in self.truncations.keys() }
            return self.truncations
        if truncated_agents is None:
            return self.truncations
        
        for agent in truncated_agents:
            if agent == '':
                self.truncations = { agent: True for agent in self.truncations.keys() }
                break
            else:
                self.truncations[agent] = True
        return self.truncations

    def parse_infos(self, info_protos: List[InfoProto]=None):
        if info_protos is None:
            return self.infos
        
        env_info_proto = None
        for info_proto in info_protos:
            if info_proto.agent != '':
                # TODO v: when to renew list?
                if info_proto.agent not in self.infos:
                    # if the agent info was deleted, initialize a new one for the agent
                    self.infos[info_proto.agent] = { 'env': [], 'agent': [] }
                self.infos[info_proto.agent]['agent'] = []
                for instance_proto in info_proto.infos:
                    self.infos[info_proto.agent]['agent'].append(space.from_proto(instance_proto))
            else:
                env_info_proto = info_proto
        if env_info_proto is not None:
            env_infos = []
            for instance_proto in env_info_proto.infos:
                env_infos.append(space.from_proto(instance_proto))
            for info in self.infos.values():
                info['env'] = env_infos

        return self.infos