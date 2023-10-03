import asyncio
import time
from typing import Any, Dict, List, Tuple, Union

from gymnasium.spaces import Space

from gymize import space
from gymize.channel import Channel, Content
from gymize.lanch import launch_env
from gymize.render import parse_rendering
from gymize.proto.gymize_pb2 import GymizeProto, ActionProto, ObservationProto, RewardProto, InfoProto
from gymize.proto.render_pb2 import ViewProto, VideoProto

class Bridge:
    def __init__(
        self,
        env_name: str,
        file_name: str=None,
        action_spaces: Dict[str, Space]={},
        observation_spaces: Dict[str, Space]={},
        reward_ranges: Dict[str, Tuple[float, float]]={},
        agents: List[str]=[],
        render_mode: str=None,
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

        self.render_mode = render_mode
        self.rendering = {}
        self.video_paths = {}
        
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
    
    def begin_render(self, view_configs: Dict[str, Dict[str, Any]]):
        # view_configs: { name: { is_single_frame, width, height, fullscreen } }
        gymize_proto = GymizeProto()
        for name, config in view_configs.items():
            view_config = ViewProto(
                name=name,
                is_single_frame=bool(config['is_single_frame'] or False),
                screen_width=int(config['screen_width'] or -1),
                screen_height=int(config['screen_height'] or -1),
                fullscreen=bool(config['fullscreen'] or False)
            )
            gymize_proto.rendering.view_configs.append(view_config)
            gymize_proto.rendering.begin_views.append(name)
        self.send_gymize_message(gymize_proto)
    
    def end_render(self, names: List[str]):
        gymize_proto = GymizeProto()
        for name in names:
            gymize_proto.rendering.end_views.append(name)
        self.send_gymize_message(gymize_proto)
    
    def render_all(self, names: List[str], video_paths: Dict[str, str]={}):
        for name, path in video_paths.items():
            self.video_paths[name] = path
        
        gymize_proto = GymizeProto()
        for name in names:
            gymize_proto.rendering.request_views.append(name)
        self.send_gymize_message(gymize_proto)

        self.wait_gymize_message(wait_render=True)

        renders = self.rendering
        self.rendering = {}

        return renders

    def render(self, name: str=None) -> None:
        names = [] if name is None else [ name ]
        renders = self.render_all(names=names)
        if len(renders) == 0:
            return None
        else:
            return next(iter(renders.values()))

    def close(self) -> None:
        self.channel.close_sync()
    
    ###########################################################################
    #                                 Utils                                   #
    ###########################################################################

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
    
    def wait_gymize_message(self, wait_agents: List[str]=[], wait_render=False) -> None:
        # agents: the agents that have to especially wait for, blocking
        # TODO v: waiting for agents
        while True:
            content, done = self.channel.wait_message(id='_gym_', polling_secs=self.update_seconds)
            observations, rewards, terminations, truncations, infos, rendering = self.parse_message(content=content, done=done)
            if self.is_agents_collected(agents=wait_agents):
                for agent in wait_agents:
                    self.responsed[agent] = False
                break
            if not wait_render or rendering is not None:
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
        rendering = None

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
            rendering = self.parse_rendering(gymize_proto.rendering.videos)
        
        return observations, rewards, terminations, truncations, infos, rendering

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
    
    def parse_rendering(self, video_protos: List[VideoProto]=None):
        if video_protos is None:
            return None
        
        if len(video_protos) == 0:
            self.rendering = {}
            return None

        self.rendering = {}
        for video_proto in video_protos:
            video_path = None
            if video_proto.name in self.video_paths:
                video_path = self.video_paths[video_proto.name]
            self.rendering[video_proto.name] = parse_rendering(video_proto=video_proto, render_mode=self.render_mode, video_path=video_path)
        return self.rendering


class BridgeChannel:
    def __init__(self, bridge: Bridge):
        self.bridge = bridge
    
    '''
    id == '' means for the root channel
    '''
    
    def on_message(self, id=''):
        '''
        Example ussage:

        channel = Channel(...)
        ...

        @channel.on_message(<id>)
        def on_message(msg):
            print(msg)
        
        or

        channel.on_message(<id>)(on_message)

        or

        channel.on_message(<id>)(lambda msg: print(msg))
        
        '''
        return self.bridge.channel.on_message(id=id)
    
    def on_request(self, id=''):
        '''
        Example ussage:

        channel = Channel(...)
        ...

        @channel.on_request(<id>)
        def on_request(req):
            print(req)
        
        or

        channel.on_message(<id>)(on_request)

        or

        channel.on_request(<id>)(lambda req: print(req))
        
        '''
        return self.bridge.channel.on_request(id=id)
    
    def off_message(self, id=''):
        '''
        remove all the message callbacks associated to the id
        '''
        self.bridge.channel.off_message(id=id)
    
    def off_request(self, id=''):
        '''
        remove all the request callbacks associated to the id
        '''
        self.bridge.channel.off_request(id=id)
    
    async def tell_async(self, id: str, content: Union[Content, str, bytes]) -> None:
        await self.bridge.channel.tell_async(id=id, content=content)
    
    def tell_sync(self, id: str, content: Union[Content, str, bytes]) -> None:
        self.bridge.channel.tell_sync(id=id, content=content)
    
    async def broadcast_async(self, content: Union[Content, str, bytes]) -> None:
        await self.bridge.channel.broadcast_async(content=content)
    
    def broadcast_sync(self, content: Union[Content, str, bytes]) -> None:
        self.bridge.channel.broadcast_sync(content=content)

    async def ask_async(self, id: str, content: Union[Content, str, bytes]) -> Content:
        return await self.bridge.channel.ask_async(id=id, content=content)

    def ask_sync(self, id: str, content: Union[Content, str, bytes]) -> asyncio.Future:
        return self.bridge.channel.ask_sync(id=id, content=content)
    
    def has_recv(self) -> bool:
        '''
        check if receive message, response
        :return: done
        '''
        return self.bridge.channel.has_recv()
    
    def wait(self, polling_secs: float=0.001) -> bool:
        '''
        wait until receive message, response or channel closed
        :return: done
        '''
        return self.bridge.channel.wait(polling_secs=polling_secs)
    
    def has_message(self, id: str) -> bool:
        '''
        check if receive message
        :return: done
        '''
        return self.bridge.channel.has_message(id=id)

    def take_message(self, id: str) -> Content:
        '''
        take message and pop from the queue
        :return: content
        '''
        return self.bridge.channel.take_message(id=id)
    
    def wait_message(self, id: str, polling_secs: float=0.001) -> Tuple[Content, bool]:
        '''
        after received a message, or the channel is closed
        it will return content and whether the channel is running now
        :return: content, done
        '''
        return self.bridge.channel.wait_message(id=id, polling_secs=polling_secs)
    
    def take_response(self, response: asyncio.Future) -> Content:
        '''
        take response and remove
        :return: content
        '''
        return self.bridge.channel.take_response(response=response)

    def wait_response(self, response: asyncio.Future, polling_secs: float=0.001) -> Tuple[Content, bool]:
        '''
        after received a response, or the channel is closed
        it will return content and whether the channel is running now
        :return: content, done
        '''
        return self.bridge.channel.wait_response(response=response, polling_secs=polling_secs)