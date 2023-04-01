from queue import Queue
import threading
import time
import uuid
from typing import Dict, List, Tuple, Union
import asyncio
import websockets
from websockets.client import WebSocketClientProtocol
from websockets.extensions import permessage_deflate
from websockets.exceptions import ConnectionClosed
import socket
from gymize.signaling import SignalingServer
from gymize.proto.signaling_pb2 import Signal, SignalType, PeerType
from gymize.proto.channel_pb2 import MessageType, Content, Message

class Channel:
    def __init__(self, name: str, signaling_url='ws://localhost:50864/', mode: str='active', protocol='ws', host='localhost', port=None, peer_url=None, retry=True):
        # For Signal Server
        self.name: str = name # game name (e.g. kart)
        self.signaling_url: str = signaling_url
        self.mode: str = mode # 'active' or 'passive'
        self.protocol: str = protocol
        self.host: str = host
        self.port: int = port
        self.using_available_port = self.port is None
        self.peer_url: str = peer_url
        self.using_existing_url = self.peer_url is not None
        self.signaling_id: str = None
        self.ws_signaling: WebSocketClientProtocol = None
        # For channel
        self.retry = retry # whether to retry connection when disconnected
        self.status = None # 'connecting' or 'connected' or 'disconnected' or 'closed'
        self.ws = None # Peer websocket
        self.event_loop: asyncio.AbstractEventLoop = None # the event loop for the channel
        self.channel_stop: asyncio.Future = None # if the channel is stopped
        self.peer_stop: asyncio.Future = None # if the peer server is stoped
        self.updating = False # if the channel peer server is updating
        self.sending = False # if the channel peer server is sending messages
        self._outbox: Queue = Queue() # Queue[Message]
        self._inbox: Dict[str, Queue] = dict() # { id: Queue[bytes|str] }
        self._responses: Dict[bytes, asyncio.Future] = dict() # { uuid: Future[Content] }
        self._callbacks = {
            'open': list(),
            'message': list(),
            'request': list(),
            'error': list(),
            'signaling_disconnected': list(),
            'peer_disconnected': list(),
            'close': list()
        } # { event_name: List[function] }
        self._message_callbacks: Dict[str, List] = dict() # { id: List[function] }
        self._request_callbacks: Dict[str, List] = dict() # { id: List[function] }
    
    def running_loop(self, loop: asyncio.AbstractEventLoop=None):
        if loop is None:
            # run event loop on new thread
            loop = asyncio.new_event_loop()
            self.event_loop = loop
            self.channel_stop = self.event_loop.create_future()
            # daemon=True means the thread will be shutdown together with the main thread
            thread = threading.Thread(target=self.start_background_loop, daemon=True)
            thread.start()
            return loop
        else:
            # use the given event loop
            self.event_loop = loop
            self.channel_stop = self.event_loop.create_future()
            return loop
    
    def start_background_loop(self):
        asyncio.set_event_loop(self.event_loop)
        self.event_loop.run_until_complete(self.wait_finish())
        time.sleep(10) # To ensure all the things are completed
    
    async def wait_finish(self):
        return await self.channel_stop

    def is_running(self):
        return not self.channel_stop.done()
    
    def wait_finish_sync(self, polling_secs: float=0.001):
        while self.is_running():
            time.sleep(polling_secs)
        return self.channel_stop.result()
    
    def add_task(self, coro) -> asyncio.Future:
        if self.event_loop is None:
            self.running_loop()
        return asyncio.run_coroutine_threadsafe(coro, self.event_loop)
    
    async def connect(self, using_thread: bool=False):
        if self.status is None:
            if using_thread:
                # run signal client in another task
                self.add_task(self.signaling())
            else:
                self.event_loop = asyncio.get_running_loop()
                self.channel_stop = self.event_loop.create_future()
                self.add_task(self.signaling())
                await self.wait_finish()
        else:
            print('Channel can only connect once, use another channel instead')
        
    def connect_sync(self):
        return self.add_task(self.connect(using_thread=True))
    
    async def tell(self, id: str, data) -> None:
        msg = Message()
        msg.header.message_type = MessageType.MESSAGE_TYPE_MESSAGE
        if id is not None:
            msg.header.id = id
        if type(data) is str:
            msg.content.text = data
        else:
            msg.content.raw = bytes(data)
        await self.send(msg=msg)
    
    def tell_sync(self, id: str, data) -> None:
        return self.add_task(self.tell(id=id, data=data))
    
    async def broadcast(self, data) -> None:
        await self.tell(id=None, data=data)
    
    def broadcast_sync(self, data) -> None:
        return self.add_task(self.broadcast(data=data))

    async def ask(self, id: str, data):
        if not id in self._responses:
            self._responses[id] = Queue()
        
        msg = Message()
        msg.header.message_type = MessageType.MESSAGE_TYPE_REQUEST
        if id is not None:
            msg.header.id = id
        msg.header.uuid = uuid.uuid4().bytes
        if type(data) is str:
            msg.content.text = data
        else:
            msg.content.raw = bytes(data)
        self._responses[msg.header.uuid] = asyncio.Future()
        await self.send(msg=msg)
        content = await self._responses[msg.header.uuid]
        self._responses.pop(msg.header.uuid, None)
        return content

    def ask_sync(self, id: str, data) -> asyncio.Future:
        return self.add_task(self.ask(id=id, data=data))
    
    async def send(self, msg: Message) -> None:
        self._outbox.put(msg)
        await self.send_flush() # flush the outbox because the outbox is not empty now
    
    def send_sync(self, msg: Message) -> None:
        return self.add_task(self.send(msg=msg))

    async def send_flush(self) -> None:
        if self.sending:
            return
        self.sending = True
        while not self._outbox.empty():
            if self.ws is not None and self.ws.open:
                msg = self._outbox.get()
                await self.ws.send(msg.SerializeToString())
            else:
                break
        self.sending = False
    
    def recv(self, data) -> None:
        msg = Message()
        msg.ParseFromString(data)
        content = None
        if msg.content.HasField('raw'):
            content = msg.content.raw
        elif msg.content.HasField('text'):
            content = msg.content.text
        
        if msg.header.message_type == MessageType.MESSAGE_TYPE_MESSAGE:
            if msg.header.id:
                if not msg.header.id in self._inbox:
                    self._inbox[msg.header.id] = Queue()
                self._inbox[msg.header.id].put(content)
                self.trigger_message(msg.header.id, content)
            else:
                for id in self._inbox:
                    self._inbox[id].put(content)
                self.trigger('message', content)
        elif msg.header.message_type == MessageType.MESSAGE_TYPE_REQUEST:
            request = Request(id=msg.header.id, uuid=msg.header.uuid, data=content, channel=self)
            if msg.header.id:
                self.trigger_request(msg.header.id, request)
            else:
                self.trigger('request', request)
        elif msg.header.message_type == MessageType.MESSAGE_TYPE_RESPONSE:
            self._responses[msg.header.uuid].set_result(content)
    
    def has_recv(self) -> bool:
        '''
        check if receive message, response
        :return: done
        '''
        for id in self._inbox:
            if not self._inbox[id].empty():
                return True
        for uuid in self._responses:
            if self._responses[uuid].done():
                return True
        return False
    
    def wait(self, polling_secs: float=0.001):
        '''
        wait until receive message, response or channel closed
        :return: done
        '''
        while not self.has_recv() and self.is_running():
            time.sleep(polling_secs)
        return not self.is_running()
    
    def has_message(self, id: str) -> bool:
        '''
        check if receive message
        :return: done
        '''
        if not id in self._inbox:
            self._inbox[id] = Queue()
        return not self._inbox[id].empty()
    
    def wait_message(self, id: str, polling_secs: float=0.001) -> Tuple[Union[bytes, str], bool]:
        '''
        after received a message, or the channel is closed
        it will return data and whether the channel is running now
        :return: data, done
        '''
        while not self.has_message(id=id) and self.is_running():
            time.sleep(polling_secs)
        data = None
        if not self._inbox[id].empty():
            data = self._inbox[id].get()
        return data, not self.is_running()

    def wait_response(self, response: asyncio.Future, polling_secs: float=0.001) -> Tuple[Union[bytes, str], bool]:
        '''
        after received a response, or the channel is closed
        it will return data and whether the channel is running now
        :return: data, done
        '''
        while not response.done() and self.is_running():
            time.sleep(polling_secs)
        data = None
        if response.done():
            data = response.result()
        return data, not self.is_running()
    
    async def pause(self):
        if self.ws_signaling is not None and self.ws_signaling.open:
            await self.ws_signaling.close()

    def pause_sync(self):
        return self.add_task(self.pause())
    
    async def resume(self):
        '''
        resume the signal connection with current id
        '''
        await self.signaling(is_resume=True)
    
    def resume_sync(self):
        return self.add_task(self.resume())
    
    async def close(self):
        self.status = 'closed'
        if self.ws_signaling is not None and self.ws_signaling.open:
            signal = Signal()
            signal.signal_type = SignalType.SIGNAL_TYPE_CLOSE
            signal.id = self.signaling_id
            await self.ws_signaling.send(signal.SerializeToString())
        if self.ws is not None and self.ws.open:
            await self.ws.close()
        self.ws = None
    
    def close_sync(self):
        return self.add_task(self.close())
    
    ###########################################################################
    #                        Event-driven style part                          #
    ###########################################################################

    def on(self, event_name):
        '''
        event_name: one of 'open', 'message', 'error', 'close'
        
        Example ussage:

        channel = Channel(...)
        ...

        @channel.on('message')
        def on_message(msg):
            print(msg)
        
        or

        channel.on('message')(on_message)

        or

        channel.on('message')(lambda msg: print(msg))
        
        '''
        def decorate(callback):
            if not event_name in self._callbacks:
                self._callbacks[event_name] = list()
            self._callbacks[event_name].append(callback)

        return decorate

    def on_message(self, id):
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
        def decorate(callback):
            if not id in self._message_callbacks:
                self._message_callbacks[id] = list()
            self._message_callbacks[id].append(callback)

        return decorate
    
    def on_request(self, id):
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
        def decorate(callback):
            if not id in self._request_callbacks:
                self._request_callbacks[id] = list()
            self._request_callbacks[id].append(callback)

        return decorate

    def trigger(self, event_name, *args, **kwargs):
        if event_name in self._callbacks:
            for callback in self._callbacks[event_name]:
                if asyncio.iscoroutinefunction(callback):
                    self.add_task(callback(*args, **kwargs))
                else:
                    callback(*args, **kwargs)
    
    def trigger_message(self, id, *args, **kwargs):
        if id in self._message_callbacks:
            for callback in self._message_callbacks[id]:
                if asyncio.iscoroutinefunction(callback):
                    self.add_task(callback(*args, **kwargs))
                else:
                    callback(*args, **kwargs)
    
    def trigger_request(self, id, *args, **kwargs):
        if id in self._request_callbacks:
            for callback in self._request_callbacks[id]:
                if asyncio.iscoroutinefunction(callback):
                    self.add_task(callback(*args, **kwargs))
                else:
                    callback(*args, **kwargs)

    def off(self, event_name=None):
        '''
        remove all the callbacks associated to the event_name
        '''
        if event_name in self._callbacks:
            self._callbacks[event_name] = list()
        if event_name in self._callbacks:
            self._callbacks[event_name].clear()

    def off_message(self, id=None):
        '''
        remove all the message callbacks associated to the id
        '''
        if id in self._message_callbacks:
            self._message_callbacks[id] = list()
        if id in self._message_callbacks:
            self._message_callbacks[id].clear()
    
    def off_request(self, id=None):
        '''
        remove all the request callbacks associated to the id
        '''
        if id in self._request_callbacks:
            self._request_callbacks[id] = list()
        if id in self._request_callbacks:
            self._request_callbacks[id].clear()

    ###########################################################################
    #                           Signal client  part                           #
    ###########################################################################

    async def ensure_signaling_server_opened(self):
        try:
            async with websockets.connect(self.signaling_url):
                pass
        except OSError:
            # The given signaling server is not available, create a new one
            server = SignalingServer()
            self.add_task(server.ws_server(host='localhost', port=50864, stop=self.channel_stop)) # gymize_signaling = 50864
            await asyncio.sleep(1)
            self.signaling_url = 'ws://localhost:50864'

    async def signaling(self, is_resume=False):
        self.status = 'connecting'

        await self.ensure_signaling_server_opened()

        async with websockets.connect(self.signaling_url, ping_timeout=None) as websocket:
            closed = None
            print(f'Connected to Signal Server: {self.signaling_url}')
            self.ws_signaling = websocket
            try:
                await self.signaling_start(is_resume=is_resume)

                async for msg in self.ws_signaling: # onmessage
                    await self.signaling_recv(msg)
            
            except ConnectionClosed:
                closed = True
            except Exception as error:
                closed = False
                self.trigger('error', error)
            
            # websocket is closed
            if closed != False:
                self.ws_signaling = None
                print(f'The Signal Server connection: {self.signaling_url} is closed')
                if self.status == 'closed':
                    # close the channel
                    self.trigger('close')
                    # stop the background event loop
                    if self.channel_stop is not None:
                        self.channel_stop.set_result(True)
                else:
                    self.status = 'disconnected'
                    self.trigger('signaling_disconnected')
                    if self.retry:
                        await self.resume()
    
    async def signaling_start(self, is_resume=False):
        signal = Signal()

        if not is_resume:
            # initialize
            signal.signal_type = SignalType.SIGNAL_TYPE_INIT
            signal.name = self.name
        else:
            # resume
            signal.signal_type = SignalType.SIGNAL_TYPE_RESUME
            signal.id = self.signaling_id
        
        if self.mode == 'active':
            signal.peer_type = PeerType.PEER_TYPE_ACTIVE
        elif self.mode == 'passive':
            signal.peer_type = PeerType.PEER_TYPE_PASSIVE
        if self.ws_signaling is not None and self.ws_signaling.open:
            await self.ws_signaling.send(signal.SerializeToString())
    
    async def signaling_recv(self, msg):
        signal = Signal()
        signal.ParseFromString(msg)

        # switch by signal type
        if signal.signal_type == SignalType.SIGNAL_TYPE_INIT:
            # set signaling id given by the signal server
            self.signaling_id = signal.id
            if self.mode == 'active':
                await self.update() # get a Peer Server

        elif signal.signal_type == SignalType.SIGNAL_TYPE_UPDATE:
            if self.mode == 'active':
                await self.update() # get a Peer Server
            if self.mode == 'passive':
                # connect to Peer Server, in another task
                self.add_task(self.ws_client(url=signal.url))
        
        elif signal.signal_type == SignalType.SIGNAL_TYPE_CLOSE:
            self.status = 'closed'
            if self.ws_signaling is not None and self.ws_signaling.open:
                await self.ws_signaling.close()
            self.ws_signaling = None
    
    ###########################################################################
    #                            Peer channel part                            #
    ###########################################################################

    async def update(self, waiting_secs: float=1):
        '''
        Establish a new peer connection and replace the old one
        '''
        # Lock when updating
        if self.updating:
            return
        else:
            self.updating = True
            if self.ws is not None and self.ws.open:
                await self.ws.close()
        
        signal = Signal()
        signal.signal_type = SignalType.SIGNAL_TYPE_UPDATE
        signal.id = self.signaling_id
        
        if self.mode == 'active':
            # get Peer Server
            if not self.using_existing_url:
                self.peer_url = await self.create_peer_server(waiting_secs)
            signal.url = self.peer_url # send url (protocol://host:port) to the other peer
        elif self.mode == 'passive':
            # Ask active peer to raise an update signal
            pass
        
        # send update information to the other peer
        if self.ws_signaling is not None and self.ws_signaling.open:
            await self.ws_signaling.send(signal.SerializeToString())
    
    def update_sync(self, waiting_secs: float=1):
        return self.add_task(self.update(waiting_secs=waiting_secs))
    
    async def create_peer_server(self, waiting_secs: float=1):
        # Choose an available port by the system
        sock = socket.socket()
        sock.bind(('', 0))
        if self.using_available_port:
            self.port = int(sock.getsockname()[1])
        # start a websocket server in another task
        self.peer_url = f'{self.protocol}://{self.host}:{self.port}'
        self.add_task(self.ws_server(host=self.host, port=self.port))
        await asyncio.sleep(waiting_secs)
        return self.peer_url
    
    async def ws_server(self, host, port):
        self.peer_stop = asyncio.Future() # you can set a value to it
        extensions = [permessage_deflate.ServerPerMessageDeflateFactory()]
        async with websockets.serve(self.ws_server_recv, host, port, extensions=extensions, ping_timeout=None):
            print(f'Start Peer Server: {self.peer_url}')
            
            await self.peer_stop # run forever until stop is set
            print(f'The Peer connection: {self.peer_url} is closed')
            if self.status != 'closed':
                self.status = 'disconnected'
                self.trigger('peer_disconnected')
                if self.retry:
                    await self.update()

    async def ws_server_recv(self, websocket):
        closed = None
        self.ws = websocket
        self.updating = False
        self.status = 'connected'
        await self.send_flush() # flush the outbox after connection is connected
        self.trigger('open')
        try:
            async for msg in websocket: # onmessage
                self.recv(msg)
            
        except ConnectionClosed:
            closed = True
        except Exception as error:
            closed = False
            self.trigger('error', error)
        
        # websocket is closed
        if closed != False:
            self.ws = None
            self.peer_stop.set_result(True)

    async def ws_client(self, url):
        extensions = [permessage_deflate.ClientPerMessageDeflateFactory()]
        async with websockets.connect(url, extensions=extensions, ping_timeout=None) as websocket:
            closed = None
            self.peer_url = url
            print(f'Connected to Peer Server: {self.peer_url}')
            self.ws = websocket
            self.updating = False
            try:
                self.status = 'connected'
                await self.send_flush() # flush the outbox after connection is connected
                self.trigger('open')
                
                async for msg in websocket: # onmessage
                    self.recv(msg)
                
            except ConnectionClosed:
                closed = True
            except Exception as error:
                closed = False
                self.trigger('error', error)
            
            # websocket is closed
            if closed != False:
                self.ws = None
                print(f'The Peer connection: {self.peer_url} is closed')
                if self.status != 'closed':
                    self.status = 'disconnected'
                    self.trigger('peer_disconnected')
                    if self.retry:
                        await self.update()
    
class Request:
    def __init__(self, id: str, uuid: str, data, channel: Channel):
        self.id = id
        self.uuid = uuid
        self.data = data
        self.channel = channel
    
    async def reply(self, data):
        msg = Message()
        msg.header.message_type = MessageType.MESSAGE_TYPE_RESPONSE
        if self.id is not None:
            msg.header.id = self.id
        msg.header.uuid = self.uuid
        if type(data) is str:
            msg.content.text = data
        else:
            msg.content.raw = bytes(data)
        await self.channel.send(msg=msg)
    
    def reply_sync(self, data):
        self.channel.add_task(self.reply(data=data))


# TODO: 改名為 Gymize, 刪除 PActor
# TODO: 在哪裡啟動 Unity, VirtualGL
# TODO: 了解 gym 如何開啟多環境，可以參考 ML-Agents: https://github.com/Unity-Technologies/ml-agents/tree/main/ml-agents-envs/mlagents_envs/envs