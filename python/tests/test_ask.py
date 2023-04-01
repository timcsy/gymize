'''
$ python test_ask.py active
Connected to Signal Server: ws://localhost:50864/
Start Peer Server: ws://localhost:57444
> f
> r
> exit
The Signal Server connection: ws://localhost:50864/ is closed
The Peer connection: ws://localhost:57444 is closed
'''

'''
$ python test_ask.py passive
Connected to Signal Server: ws://localhost:50864/
Connected to Peer Server: ws://localhost:57444
id: agent1, uuid: b'~d5\xa0\nlA\xba\x8c\x980\x8a\xea\xc5\xbd0'
> This is an Echo Bot ~~~
> f
id: agent1, uuid: b'\xcd\x1c,?\r\x08BK\xb1\x99(\x950\xc1\x88\xf7'
> r
> exit
The Peer connection: ws://localhost:57444 is closed
The Signal Server connection: ws://localhost:50864/ is closed
'''

import asyncio
import sys

from gymize.channel import Channel, Request


if __name__ == '__main__':
    mode = 'active'
    if len(sys.argv) > 1:
        mode = sys.argv[1]
    
    channel = Channel('kart', signaling_url='ws://localhost:50864/', mode=mode)

    @channel.on('open')
    async def on_open():
        if channel.mode == 'active':
            text = 'This is an Echo Bot ~~~'
            response = None
            while response != 'exit':
                response = await channel.ask('agent1', text)
                print(f'> {response}')
                await asyncio.sleep(1)
                text = input('> ')
    
    @channel.on_request('agent1')
    async def on_request(req: Request):
        print(f'id: {req.id}, uuid: {req.uuid}')
        print(f'> {req.data}')

        await asyncio.sleep(1)
        text = input('> ')
        await req.reply(text)
        if text == 'exit':
            await channel.close()
            channel.off('message')
        elif text == 'pause':
            await channel.pause()
        elif text == 'disconnect':
            await channel.ws.close()
        elif text == 'update':
            await channel.update()
    
    # Using multi thread

    channel.connect_sync()
    print(channel.wait_finish_sync())

    # Using single thread

    # loop = asyncio.new_event_loop()
    # asyncio.set_event_loop(loop)
    # asyncio.run(channel.connect())