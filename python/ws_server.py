import asyncio
import websockets
from websockets.extensions import permessage_deflate
from proto.space_pb2 import Data
import queue

msg_queue = queue.Queue()

async def ws_recv(websocket):
    msg = await websocket.recv()
    if type(msg) == bytes:
        p_msg = Data()
        p_msg.ParseFromString(msg)
        print(p_msg)
        blob = p_msg.SerializeToString()
        await websocket.send(blob)
    elif type(msg) == str:
        print(msg)
        await websocket.send(f"From server: {msg}")

async def ws_server(host="localhost", port=8080):
    extensions = [permessage_deflate.ServerPerMessageDeflateFactory()]
    async with websockets.serve(ws_recv, host, port, extensions=extensions):
        await asyncio.Future()  # run forever

def run_ws_server(host="localhost", port=8080):
    loop = asyncio.new_event_loop()
    asyncio.set_event_loop(loop)
    asyncio.run(ws_server(host=host, port=port))

if __name__ == "__main__":
    run_ws_server(host="localhost", port=8080)