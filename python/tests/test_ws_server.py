import asyncio
import queue
import websockets
from websockets.extensions import permessage_deflate

from gymize.proto.space_pb2 import InstanceProto, CompressionTypeProto
import gymize.space as space

msg_queue = queue.Queue()

async def ws_recv(websocket):
    msg = await websocket.recv()
    if type(msg) == bytes:
        data = InstanceProto()
        data.ParseFromString(msg)
        for key in data.dict:
            print(key)
            image = data.dict[key].image
            img = space.image_to_ndarray(image)
            print(img.shape)
            print(img)
            if (image.compression_type == CompressionTypeProto.COMPRESSION_TYPE_PROTO_PNG):
                space.save_image(img, 'img.png')
            elif (image.compression_type == CompressionTypeProto.COMPRESSION_TYPE_PROTO_JPG):
                space.save_image(img, 'img.jpg')
        # print(data)
        blob = data.SerializeToString()
        await websocket.send('received image')
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