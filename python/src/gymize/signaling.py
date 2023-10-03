import asyncio
from queue import Queue
from typing import Dict
import uuid
import websockets
from websockets.server import WebSocketServerProtocol

from gymize.proto.signaling_pb2 import SignalProto, SignalTypeProto, PeerTypeProto

class Connection:
    def __init__(self, ws_active, ws_passive, id=None):
        if id is None:
            self.id = str(uuid.uuid4())
        self.active = ws_active
        self.passive = ws_passive

class SignalingServer:
    def __init__(self):
        self.active_queue: Dict[str, Queue] = dict() # { name: WebSocket }
        self.passive_queue: Dict[str, Queue] = dict() # { name: WebSocket }
        self.ws_connections: Dict[WebSocketServerProtocol, Connection] = dict() # { WebSocket: Connection }
        self.connections: Dict[str, Connection] = dict() # { id: Connection }
    
    def run(self, host='127.0.0.1', port=50864): # gymize_signaling = 50864
        loop = asyncio.new_event_loop()
        asyncio.set_event_loop(loop)
        asyncio.run(self.ws_server(host=host, port=port))
    
    async def ws_server(self, host, port, stop: asyncio.Future=None):
        async with websockets.serve(self.ws_recv, host, port, ping_timeout=None):
            print(f'Start Signal Server: ws://{host}:{port}')
            if stop is None:
                stop = asyncio.Future()
            await stop # run forever
     
    async def ws_recv(self, websocket):
        try:
            async for msg in websocket: # onmessage
                # convert msg to Protobuf
                signal = SignalProto()
                signal.ParseFromString(msg)

                # switch by signal type
                if signal.signal_type == SignalTypeProto.SIGNAL_TYPE_PROTO_INIT:
                    await self.start(websocket, signal.name, signal.peer_type)
                elif signal.signal_type == SignalTypeProto.SIGNAL_TYPE_PROTO_UPDATE:
                    await self.update(websocket, signal)
                elif signal.signal_type == SignalTypeProto.SIGNAL_TYPE_PROTO_RESUME:
                    self.resume(websocket, signal)
                elif signal.signal_type == SignalTypeProto.SIGNAL_TYPE_PROTO_CLOSE:
                    await self.close(websocket, signal.id)

        except Exception as e:
            print(e)
        
        # websocket is closed
        self.remove(websocket)
    

    async def start(self, ws, name, peer_type):
        # initialize queue
        if name not in self.active_queue:
            self.active_queue[name] = Queue()
        if name not in self.passive_queue:
            self.passive_queue[name] = Queue()
        
        if peer_type == PeerTypeProto.PEER_TYPE_PROTO_ACTIVE:
            # put ws in the queue
            self.active_queue[name].put(ws)
            # wait until someone comes
            while self.passive_queue[name].empty():
                await asyncio.sleep(0.001)
            ws_passive = self.passive_queue[name].get()
            # initialize connection
            conn = Connection(ws_active=ws, ws_passive=ws_passive)
            self.connections[conn.id] = conn
            self.ws_connections[conn.active] = conn
            self.ws_connections[conn.passive] = conn
            # send initialization signal with id (uuid)
            signal = SignalProto()
            signal.signal_type = SignalTypeProto.SIGNAL_TYPE_PROTO_INIT
            signal.id = conn.id
            if conn.active.open:
                await conn.active.send(signal.SerializeToString())
            if conn.passive.open:
                await conn.passive.send(signal.SerializeToString())
            print(f'Connection: {name}, is initialized with id: {conn.id}')

        elif peer_type == PeerTypeProto.PEER_TYPE_PROTO_PASSIVE:
            # put ws in the queue
            self.passive_queue[name].put(ws)
    
    async def update(self, websocket, signal):
        conn = self.ws_connections[websocket]
        if conn is not None:
            if conn.active == websocket:
                if conn.passive is not None and conn.passive.open:
                    await conn.passive.send(signal.SerializeToString())
                    print(f'Connection id: {signal.id}, is using channel: {signal.url}')
            elif conn.passive == websocket:
                if conn.active is not None and conn.active.open:
                    await conn.active.send(signal.SerializeToString())

    def resume(self, websocket, signal):
        '''
        resume the connection with same signal id
        '''
        if not signal.id in self.connections:
            conn = Connection(ws_active=None, ws_passive=None, id=signal.id)
            self.connections[signal.id] = conn
        conn = self.connections[signal.id]
        if signal.peer_type == PeerTypeProto.PEER_TYPE_PROTO_ACTIVE:
            conn.active = websocket
        elif signal.peer_type == PeerTypeProto.PEER_TYPE_PROTO_PASSIVE:
            conn.passive = websocket
        self.ws_connections[websocket] = conn
    
    async def close(self, websocket, id):
        '''
        ask another peer to close, and remove signal id
        '''
        conn = None
        if websocket in self.ws_connections:
            conn = self.ws_connections[websocket]
        self.ws_connections.pop(websocket, None)
        # remove connection
        ws = None
        if conn is not None:
            if conn.active == websocket:
                ws = conn.passive
            elif conn.passive == websocket:
                ws = conn.active
            self.connections.pop(conn.id, None)
        # send close signal peers
        signal = SignalProto()
        signal.signal_type = SignalTypeProto.SIGNAL_TYPE_PROTO_CLOSE
        signal.id = id
        if ws is not None and ws.open:
            await ws.send(signal.SerializeToString())
        if websocket.open:
            await websocket.send(signal.SerializeToString())
        
        print(f'Connection id: {signal.id}, is closed')
    
    def remove(self, websocket):
        '''
        remove a closed connection, but not dealing with signal id
        '''
        conn = None
        if websocket in self.ws_connections:
            conn = self.ws_connections[websocket]
        self.ws_connections.pop(websocket, None)
        # remove in connection
        if conn is not None:
            if conn.active == websocket:
                conn.active = None
            elif conn.passive == websocket:
                conn.passive = None


if __name__ == '__main__':
    server = SignalingServer()
    server.run(host='127.0.0.1', port=50864) # gymize_signaling = 50864