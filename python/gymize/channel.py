import asyncio
from queue import Queue
import socket
import threading
import time
from typing import Dict, List, Tuple, Union
import uuid
import websockets
from websockets.client import WebSocketClientProtocol
from websockets.exceptions import ConnectionClosed
from websockets.extensions import permessage_deflate

from gymize.proto.channel_pb2 import MessageTypeProto, MessageProto
from gymize.proto.signaling_pb2 import SignalProto, SignalTypeProto, PeerTypeProto
from gymize.signaling import SignalingServer

class Content:
    def __init__(self, content: Union[bytes, str]):
        self.content = content
        self.text = None
        self.raw = None
        if type(content) is str:
            self.text = content
        if type(content) is bytes:
            self.raw = content
        
    def is_text(self) -> bool:
        return self.text is not None

    def is_binary(self) -> bool:
        return self.raw is not None

    def __str__(self) -> str:
        if self.is_text():
            return self.text
        elif self.is_binary():
            return str(self.raw)
        else:
            return str(self.content)

class Channel:
    def __init__(self, name: str, mode: str='active', signaling_url='ws://localhost:50864/', protocol='ws', host='localhost', port=None, peer_url=None, retry=True):
        # For Signal Server
        self.name: str = name # game name (e.g. kart)
        self.mode: str = mode # 'active' or 'passive'
        self._signaling_url: str = signaling_url
        self._protocol: str = protocol
        self._host: str = host
        self._port: int = port
        self._using_available_port = self._port is None
        self._peer_url: str = peer_url
        self._using_existing_url = self._peer_url is not None
        self._signaling_id: str = None
        self._ws_signaling: WebSocketClientProtocol = None
        # For channel
        self.status = None # 'connecting' or 'connected' or 'disconnected' or 'closed'
        self._retry = retry # whether to retry connection when disconnected
        self._ws_peer = None # Peer websocket
        self._event_loop: asyncio.AbstractEventLoop = None # the event loop for the channel
        self._channel_stop: asyncio.Future = None # if the channel is stopped
        self._peer_stop: asyncio.Future = None # if the peer server is stoped
        self._updating = False # if the channel peer server is updating
        self._sending = False # if the channel peer server is sending messages
        self._outbox: Queue = Queue() # Queue[MessageProto]
        self._inbox: Dict[str, Queue] = {
            '': Queue()
        } # { id: Queue[Content] }
        self._responses: Dict[bytes, asyncio.Future] = dict() # { uuid: Future[Content] }
        self._callbacks = {
            'open': list(),
            'error': list(),
            'signaling_disconnected': list(),
            'peer_disconnected': list(),
            'close': list()
        } # { event_name: List[function] }
        self._message_callbacks: Dict[str, List] = {
            '': list()
        } # { id: List[function] }
        self._request_callbacks: Dict[str, List] = {
            '': list()
        } # { id: List[function] }
    
    ###########################################################################
    #                              asyncio part                               #
    ###########################################################################
    
    def running_loop(self, loop: asyncio.AbstractEventLoop=None):
        '''
        set the event loop, or create a new one in a new thread
        '''
        if loop is None:
            # run event loop on new thread
            loop = asyncio.new_event_loop()
            self._event_loop = loop
            self._channel_stop = self._event_loop.create_future()
            # daemon=True means the thread will be shutdown together with the main thread
            thread = threading.Thread(target=self._start_background_loop, daemon=True)
            thread.start()
            return loop
        else:
            # use the given event loop
            self._event_loop = loop
            self._channel_stop = self._event_loop.create_future()
            return loop
    
    def _start_background_loop(self):
        '''
        run until the channel is stopped
        '''
        asyncio.set_event_loop(self._event_loop)
        self._event_loop.run_until_complete(self.wait_finish_async())
        time.sleep(10) # To ensure all the things are completed
    
    async def wait_finish_async(self):
        return await self._channel_stop

    def is_running(self):
        return not self._channel_stop.done()
    
    def wait_finish_sync(self, polling_secs: float=0.001):
        while self.is_running():
            time.sleep(polling_secs)
        return self._channel_stop.result()
    
    def add_task(self, coro) -> asyncio.Future:
        '''
        add a task to the channel event loop, it become syncronized
        '''
        if self._event_loop is None:
            self.running_loop()
        return asyncio.run_coroutine_threadsafe(coro, self._event_loop)

    ###########################################################################
    #                          Channel function part                          #
    ###########################################################################
    
    async def connect_async(self, using_thread: bool=False):
        if self.status is None:
            if using_thread:
                # run signal client in another task
                self.add_task(self._signaling())
            else:
                self._event_loop = asyncio.get_running_loop()
                self._channel_stop = self._event_loop.create_future()
                self.add_task(self._signaling())
                await self.wait_finish_async()
        else:
            print('Channel can only connect once, use another channel instead')
        
    def connect_sync(self):
        return self.add_task(self.connect_async(using_thread=True))
    
    async def tell_async(self, id: str, content: Union[Content, str, bytes]) -> None:
        msg = MessageProto()
        msg.header.message_type = MessageTypeProto.MESSAGE_TYPE_PROTO_MESSAGE
        if id is not None:
            msg.header.id = id
        
        if type(content) is Content:
            if content.is_text():
                msg.content.text = content.text
            elif content.is_binary():
                msg.content.raw = bytes(content.raw)
            else:
                msg.content.raw = bytes(content.content)
        if type(content) is str:
            msg.content.text = content
        else:
            msg.content.raw = bytes(content)
        
        await self.send_async(msg=msg)
    
    def tell_sync(self, id: str, content: Union[Content, str, bytes]) -> None:
        return self.add_task(self.tell_async(id=id, content=content))
    
    async def broadcast_async(self, content: Union[Content, str, bytes]) -> None:
        await self.tell_async(id=None, content=content)
    
    def broadcast_sync(self, content: Union[Content, str, bytes]) -> None:
        return self.add_task(self.broadcast_async(content=content))

    async def ask_async(self, id: str, content: Union[Content, str, bytes]) -> Content:
        if not id in self._inbox:
            self._inbox[id] = Queue()
        
        msg = MessageProto()
        msg.header.message_type = MessageTypeProto.MESSAGE_TYPE_PROTO_REQUEST
        if id is not None:
            msg.header.id = id
        msg.header.uuid = uuid.uuid4().bytes

        if type(content) is Content:
            if content.is_text():
                msg.content.text = content.text
            elif content.is_binary():
                msg.content.raw = bytes(content.raw)
            else:
                msg.content.raw = bytes(content.content)
        if type(content) is str:
            msg.content.text = content
        else:
            msg.content.raw = bytes(content)
        
        self._responses[msg.header.uuid] = asyncio.Future()
        await self.send_async(msg=msg)
        content = await self._responses[msg.header.uuid]
        self._responses.pop(msg.header.uuid, None)
        return content

    def ask_sync(self, id: str, content: Union[Content, str, bytes]) -> asyncio.Future:
        return self.add_task(self.ask_async(id=id, content=content))
    
    async def send_async(self, msg: MessageProto) -> None:
        self._outbox.put(msg)
        await self._send_flush() # flush the outbox because the outbox is not empty now
    
    def send_sync(self, msg: MessageProto) -> None:
        return self.add_task(self.send_async(msg=msg))

    async def _send_flush(self) -> None:
        if self._sending:
            return
        self._sending = True
        while not self._outbox.empty():
            if self._ws_peer is not None and self._ws_peer.open:
                msg = self._outbox.get()
                await self._ws_peer.send(msg.SerializeToString())
            else:
                break
        self._sending = False
    
    def _recv(self, data: bytes) -> None:
        msg = MessageProto()
        msg.ParseFromString(data)
        content: Content = None
        if msg.content.HasField('raw'):
            content = Content(msg.content.raw)
        elif msg.content.HasField('text'):
            content = Content(msg.content.text)
        
        if msg.header.message_type == MessageTypeProto.MESSAGE_TYPE_PROTO_MESSAGE:
            if msg.header.id:
                if not msg.header.id in self._inbox:
                    self._inbox[msg.header.id] = Queue()

                # only put the message to the queue if there is no message listener
                if not msg.header.id in self._message_callbacks or len(self._message_callbacks[msg.header.id]) == 0:
                    self._inbox[msg.header.id].put(content)
                else:
                    self.trigger_message(msg.header.id, content)
            else:
                # id == '' means broadcast, and '' means root channel itself
                for id in self._inbox:
                    # only put the message to the queue if there is no message listener
                    if not id in self._message_callbacks or len(self._message_callbacks[id]) == 0:
                        self._inbox[id].put(content)
                    else:
                        self.trigger_message(id, content)

        elif msg.header.message_type == MessageTypeProto.MESSAGE_TYPE_PROTO_REQUEST:
            request = Request(id=msg.header.id, uuid=msg.header.uuid, content=content, channel=self)
            if msg.header.id:
                self.trigger_request(msg.header.id, request)
            else:
                self.trigger_request('', request)

        elif msg.header.message_type == MessageTypeProto.MESSAGE_TYPE_PROTO_RESPONSE:
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
    
    def wait(self, polling_secs: float=0.001) -> bool:
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

    def take_message(self, id: str) -> Content:
        '''
        take message and pop from the queue
        :return: content
        '''
        content: Content = None
        if self.has_message(id=id):
            content = self._inbox[id].get()
        return content
    
    def wait_message(self, id: str, polling_secs: float=0.001) -> Tuple[Content, bool]:
        '''
        after received a message, or the channel is closed
        it will return content and whether the channel is running now
        :return: content, done
        '''
        while not self.has_message(id=id) and self.is_running():
            time.sleep(polling_secs)
        content = self.take_message(id=id)
        return content, not self.is_running()
    
    def take_response(self, response: asyncio.Future) -> Content:
        '''
        take response and remove
        :return: content
        '''
        content: Content = None
        if response.done():
            content = response.result()
        return content

    def wait_response(self, response: asyncio.Future, polling_secs: float=0.001) -> Tuple[Content, bool]:
        '''
        after received a response, or the channel is closed
        it will return content and whether the channel is running now
        :return: content, done
        '''
        while not response.done() and self.is_running():
            time.sleep(polling_secs)
        content = self.take_response(response=response)
        return content, not self.is_running()
    
    async def pause_async(self):
        if self._ws_signaling is not None and self._ws_signaling.open:
            await self._ws_signaling.close()

    def pause_sync(self):
        return self.add_task(self.pause_async())
    
    async def resume_async(self):
        '''
        resume the signal connection with current id
        '''
        await self._signaling(is_resume=True)
    
    def resume_sync(self):
        return self.add_task(self.resume_async())
    
    async def close_async(self):
        self.status = 'closed'
        if self._ws_signaling is not None and self._ws_signaling.open:
            signal = SignalProto()
            signal.signal_type = SignalTypeProto.SIGNAL_TYPE_PROTO_CLOSE
            signal.id = self._signaling_id
            await self._ws_signaling.send(signal.SerializeToString())
        if self._ws_peer is not None and self._ws_peer.open:
            await self._ws_peer.close()
        self._ws_peer = None
    
    def close_sync(self, waiting_secs: float=1.0):
        result = self.add_task(self.close_async())
        time.sleep(waiting_secs)
        return result
    
    ###########################################################################
    #                        Event-driven style part                          #
    ###########################################################################

    '''
    id == '' means for the root channel
    '''

    def on(self, event_name):
        '''
        event_name: one of 'open', 'error', 'signaling_disconnected', 'peer_disconnected', 'close'
        
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
            if event_name == 'message':
                self.on_message(id='')(callback)
            elif event_name == 'request':
                self.on_request(id='')(callback)
            else:
                if not event_name in self._callbacks:
                    self._callbacks[event_name] = list()
                self._callbacks[event_name].append(callback)

        return decorate

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
        def decorate(callback):
            if not id in self._message_callbacks:
                self._message_callbacks[id] = list()
            self._message_callbacks[id].append(callback)

        return decorate
    
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
        def decorate(callback):
            if not id in self._request_callbacks:
                self._request_callbacks[id] = list()
            self._request_callbacks[id].append(callback)

        return decorate

    def trigger(self, event_name, *args, **kwargs):
        if event_name == 'message':
            self.trigger_message(id='',*args, **kwargs)
        elif event_name == 'request':
            self.trigger_request(id='', *args, **kwargs)
        else:
            if event_name in self._callbacks:
                for callback in self._callbacks[event_name]:
                    if asyncio.iscoroutinefunction(callback):
                        self.add_task(callback(*args, **kwargs))
                    else:
                        callback(*args, **kwargs)
    
    def trigger_message(self, id='', *args, **kwargs):
        if id in self._message_callbacks:
            for callback in self._message_callbacks[id]:
                if asyncio.iscoroutinefunction(callback):
                    self.add_task(callback(*args, **kwargs))
                else:
                    callback(*args, **kwargs)
    
    def trigger_request(self, id='', *args, **kwargs):
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
        if event_name == 'message':
            self.off_message(id='')
        elif event_name == 'request':
            self.off_request(id='')
        else:
            if event_name in self._callbacks:
                self._callbacks[event_name] = list()
            if event_name in self._callbacks:
                self._callbacks[event_name].clear()

    def off_message(self, id=''):
        '''
        remove all the message callbacks associated to the id
        '''
        if id in self._message_callbacks:
            self._message_callbacks[id] = list()
        if id in self._message_callbacks:
            self._message_callbacks[id].clear()
    
    def off_request(self, id=''):
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

    async def _ensure_signaling_server_opened(self):
        try:
            async with websockets.connect(self._signaling_url):
                pass
        except OSError:
            # The given signaling server is not available, create a new one
            server = SignalingServer()
            self.add_task(server.ws_server(host='localhost', port=50864, stop=self._channel_stop)) # gymize_signaling = 50864
            await asyncio.sleep(1)
            self._signaling_url = 'ws://localhost:50864'

    async def _signaling(self, is_resume=False):
        self.status = 'connecting'

        await self._ensure_signaling_server_opened()

        async with websockets.connect(self._signaling_url, ping_timeout=None) as websocket:
            closed = None
            print(f'Connected to Signal Server: {self._signaling_url}')
            self._ws_signaling = websocket
            try:
                await self._signaling_start(is_resume=is_resume)

                async for msg in self._ws_signaling: # onmessage
                    await self._signaling_recv(msg)
            
            except ConnectionClosed:
                closed = True
            except Exception as error:
                closed = False
                self.trigger('error', error)
            
            # websocket is closed
            if closed != False:
                self._ws_signaling = None
                print(f'The Signal Server connection: {self._signaling_url} is closed')
                if self.status == 'closed':
                    # close the channel
                    self.trigger('close')
                    # stop the background event loop
                    if self._channel_stop is not None:
                        self._channel_stop.set_result(True)
                else:
                    self.status = 'disconnected'
                    self.trigger('signaling_disconnected')
                    if self._retry:
                        await self.resume_async()
    
    async def _signaling_start(self, is_resume=False):
        signal = SignalProto()

        if not is_resume:
            # initialize
            signal.signal_type = SignalTypeProto.SIGNAL_TYPE_PROTO_INIT
            signal.name = self.name
        else:
            # resume
            signal.signal_type = SignalTypeProto.SIGNAL_TYPE_PROTO_RESUME
            signal.id = self._signaling_id
        
        if self.mode == 'active':
            signal.peer_type = PeerTypeProto.PEER_TYPE_PROTO_ACTIVE
        elif self.mode == 'passive':
            signal.peer_type = PeerTypeProto.PEER_TYPE_PROTO_PASSIVE
        if self._ws_signaling is not None and self._ws_signaling.open:
            await self._ws_signaling.send(signal.SerializeToString())
    
    async def _signaling_recv(self, msg):
        signal = SignalProto()
        signal.ParseFromString(msg)

        # switch by signal type
        if signal.signal_type == SignalTypeProto.SIGNAL_TYPE_PROTO_INIT:
            # set signaling id given by the signal server
            self._signaling_id = signal.id
            if self.mode == 'active':
                await self.update_async() # get a Peer Server

        elif signal.signal_type == SignalTypeProto.SIGNAL_TYPE_PROTO_UPDATE:
            if self.mode == 'active':
                await self.update_async() # get a Peer Server
            if self.mode == 'passive':
                # connect to Peer Server, in another task
                self.add_task(self._ws_client(url=signal.url))
        
        elif signal.signal_type == SignalTypeProto.SIGNAL_TYPE_PROTO_CLOSE:
            self.status = 'closed'
            if self._ws_signaling is not None and self._ws_signaling.open:
                await self._ws_signaling.close()
            self._ws_signaling = None
    
    ###########################################################################
    #                            Peer channel part                            #
    ###########################################################################

    async def update_async(self, waiting_secs: float=1):
        '''
        Establish a new peer connection and replace the old one
        '''
        # Lock when updating
        if self._updating:
            return
        else:
            self._updating = True
            if self._ws_peer is not None and self._ws_peer.open:
                await self._ws_peer.close()
        
        signal = SignalProto()
        signal.signal_type = SignalTypeProto.SIGNAL_TYPE_PROTO_UPDATE
        signal.id = self._signaling_id
        
        if self.mode == 'active':
            # get Peer Server
            if not self._using_existing_url:
                self._peer_url = await self._create_peer_server(waiting_secs)
            signal.url = self._peer_url # send url (protocol://host:port) to the other peer
        elif self.mode == 'passive':
            # Ask active peer to raise an update signal
            pass
        
        # send update information to the other peer
        if self._ws_signaling is not None and self._ws_signaling.open:
            await self._ws_signaling.send(signal.SerializeToString())
    
    def update_sync(self, waiting_secs: float=1):
        return self.add_task(self.update_async(waiting_secs=waiting_secs))
    
    async def _create_peer_server(self, waiting_secs: float=1):
        # Choose an available port by the system
        sock = socket.socket()
        sock.bind(('', 0))
        if self._using_available_port:
            self._port = int(sock.getsockname()[1])
        # start a websocket server in another task
        self._peer_url = f'{self._protocol}://{self._host}:{self._port}'
        self.add_task(self._ws_server(host=self._host, port=self._port))
        await asyncio.sleep(waiting_secs)
        return self._peer_url
    
    async def _ws_server(self, host, port):
        self._peer_stop = asyncio.Future() # you can set a value to it
        extensions = [permessage_deflate.ServerPerMessageDeflateFactory()]
        async with websockets.serve(self._ws_server_recv, host, port, extensions=extensions, ping_timeout=None):
            print(f'Start Peer Server: {self._peer_url}')
            
            await self._peer_stop # run forever until stop is set
            print(f'The Peer connection: {self._peer_url} is closed')
            if self.status != 'closed':
                self.status = 'disconnected'
                self.trigger('peer_disconnected')
                if self._retry:
                    await self.update_async()

    async def _ws_server_recv(self, websocket):
        closed = None
        self._ws_peer = websocket
        self._updating = False
        self.status = 'connected'
        await self._send_flush() # flush the outbox after connection is connected
        self.trigger('open')
        try:
            async for msg in websocket: # onmessage
                self._recv(msg)
            
        except ConnectionClosed:
            closed = True
        except Exception as error:
            closed = False
            self.trigger('error', error)
        
        # websocket is closed
        if closed != False:
            self._ws_peer = None
            self._peer_stop.set_result(True)

    async def _ws_client(self, url):
        extensions = [permessage_deflate.ClientPerMessageDeflateFactory()]
        async with websockets.connect(url, extensions=extensions, ping_timeout=None) as websocket:
            closed = None
            self._peer_url = url
            print(f'Connected to Peer Server: {self._peer_url}')
            self._ws_peer = websocket
            self._updating = False
            try:
                self.status = 'connected'
                await self._send_flush() # flush the outbox after connection is connected
                self.trigger('open')
                
                async for msg in websocket: # onmessage
                    self._recv(msg)
                
            except ConnectionClosed:
                closed = True
            except Exception as error:
                closed = False
                self.trigger('error', error)
            
            # websocket is closed
            if closed != False:
                self._ws_peer = None
                print(f'The Peer connection: {self._peer_url} is closed')
                if self.status != 'closed':
                    self.status = 'disconnected'
                    self.trigger('peer_disconnected')
                    if self._retry:
                        await self.update_async()

class Request:
    def __init__(self, id: str, uuid: bytes, content: Union[Content, str, bytes], channel: Channel):
        self.id = id
        self.uuid = uuid

        if type(content) is Content:
            self.content = content
        else:
            self.content = Content(content)
        
        self.channel = channel
    
    async def reply_async(self, content: Union[Content, str, bytes]):
        msg = MessageProto()
        msg.header.message_type = MessageTypeProto.MESSAGE_TYPE_PROTO_RESPONSE
        if self.id is not None:
            msg.header.id = self.id
        msg.header.uuid = self.uuid

        if type(content) is Content:
            if content.is_text():
                msg.content.text = content.text
            elif content.is_binary():
                msg.content.raw = bytes(content.raw)
            else:
                msg.content.raw = bytes(content.content)
        if type(content) is str:
            msg.content.text = content
        else:
            msg.content.raw = bytes(content)
        
        await self.channel.send_async(msg=msg)
    
    def reply_sync(self, content: Union[Content, str, bytes]):
        return self.channel.add_task(self.reply_async(content=content))