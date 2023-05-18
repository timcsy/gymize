'''
$ python test_signal.py active
Connected to Signal Server: ws://localhost:50864/
Start Peer Server: ws://localhost:58108
> This is an Echo Bot ~~~
agent1
1
> f
agent1
2
> disconnect
agent1
3
The Peer connection: ws://localhost:58108 is closed
Start Peer Server: ws://localhost:58123
> This is an Echo Bot ~~~
agent1
4
> pause
agent1
5
> r
agent1
6
> update
agent1
7
The Peer connection: ws://localhost:58123 is closed
Start Peer Server: ws://localhost:58142
> g
agent1
8
> This is an Echo Bot ~~~
agent1
9
> h
agent1
10
> h
agent1
11
> exit
agent1
12
The Peer connection: ws://localhost:58142 is closed
The Signal Server connection: ws://localhost:50864/ is closed
Channel can only connect once, use another channel instead
True
'''

'''
$ python test_signal.py passive
Connected to Signal Server: ws://localhost:50864/
Connected to Peer Server: ws://localhost:58108
> This is an Echo Bot ~~~
agent1
1
> f
> f
agent1
2
> disconnect
The Peer connection: ws://localhost:58108 is closed
Connected to Peer Server: ws://localhost:58123
> disconnect
agent1
3
> This is an Echo Bot ~~~
agent1
4
> pause
> r
> pause
agent1
5
The Signal Server connection: ws://localhost:50864/ is closed
> r
agent1
6
Connected to Signal Server: ws://localhost:50864/
> update
> g
The Peer connection: ws://localhost:58123 is closed
Connected to Peer Server: ws://localhost:58142
> update
agent1
7
> g
agent1
8
> This is an Echo Bot ~~~
agent1
9
> h
> h
> exit
> h
agent1
11
> h
agent1
11
The Peer connection: ws://localhost:58142 is closed
The Signal Server connection: ws://localhost:50864/ is closed
Channel can only connect once, use another channel instead
True
'''

import asyncio
import sys
import time

import websockets
from gymize.channel import Channel, Content


if __name__ == '__main__':
    mode = 'active'
    if len(sys.argv) > 1:
        mode = sys.argv[1]
    
    channel = Channel('kart', mode=mode, signaling_url='ws://localhost:50864/')

    @channel.on('open')
    async def on_open():
        if channel.mode == 'passive':
            # await channel.broadcast('This is an Echo Bot ~~~')
            await channel.tell_async('agent1', 'This is an Echo Bot ~~~')

    # @channel.on('message')
    # async def on_message(data):
    #     if type(data) == bytes:
    #         print(f'> {data}')
    #     elif type(data) == str:
    #         print(f'> {data}')
    #     if mode == 'active':
    #         await channel.broadcast(data)
    #     else:
    #         await asyncio.sleep(1)
    #         text = input('> ')
    #         await channel.broadcast(text)
    #         if text == 'exit':
    #             await channel.close()
    #             channel.off('message')
    #         elif text == 'pause':
    #             await channel.pause()
    #         elif text == 'disconnect':
    #             await channel.ws.close()
    #         elif text == 'update':
    #             await channel.update()
    
    @channel.on_message('agent1')
    async def on_message(content: Content):
        print(f'> {content}')
        
        for key in channel._inbox:
            print(key)
            print(channel._inbox[key].qsize())
        
        if mode == 'active':
            await channel.tell_async('agent1', content)
        else:
            await asyncio.sleep(1)
            text = input('> ')
            await channel.tell_async('agent1', text)
            if text == 'exit':
                await channel.close_async()
                channel.off('message')
            elif text == 'pause':
                await channel.pause_async()
            elif text == 'disconnect':
                await channel._ws_peer.close()
            elif text == 'update':
                await channel.update_async()
    
    @channel.on('signaling_disconnected')
    async def on_signaling_disconnected():
        # print('resume')
        # await channel.resume()
        pass
    
    @channel.on('peer_disconnected')
    async def on_peer_disconnected():
        # await channel.update()
        pass
    
    @channel.on('close')
    async def on_close():
        await channel.connect_async() # Please don't do this!
        pass
    
    # Using multi thread

    channel.connect_sync()
    print(channel.wait_finish_sync())

    # Using single thread

    # loop = asyncio.new_event_loop()
    # asyncio.set_event_loop(loop)
    # asyncio.run(channel.connect())