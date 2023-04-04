'''
$ python ask_sync.py 
Connected to Signal Server: ws://localhost:50864/
Start Peer Server: ws://localhost:59034
> f
> r
The Peer connection: ws://localhost:59034 is closed
The Signal Server connection: ws://localhost:50864/ is closed
> exit
> 
'''

'''
$ python ask.py passive
Connected to Signal Server: ws://localhost:50864/
Connected to Peer Server: ws://localhost:59034
id: agent1, uuid: b'\xa5\xf3\xc1&%\xddI\x14\x84\xfd\x04\xcc\xc6\xa5\r\xa6'
> This is an Echo Bot ~~~
> f
id: agent1, uuid: b'\x03\xa4\x0e\xcb\xad\x1dM\xc7\xbf\xca\x93)\xa6ms\x1b'
> r
> exit
The Peer connection: ws://localhost:59034 is closed
The Signal Server connection: ws://localhost:50864/ is closed
True
'''

import time

from gymize.channel import Channel

channel = Channel('kart')
channel.connect_sync()

text = 'This is an Echo Bot ~~~'
while channel.is_running():
    response = channel.ask_sync('agent1', text)
    while not response.done():
        time.sleep(0.1)
    result = response.result()
    print(f'> {result}')
    text = input('> ')