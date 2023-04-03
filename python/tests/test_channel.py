'''
$ python test_channel.py 
Connected to Signal Server: ws://localhost:50864/
Start Peer Server: ws://localhost:57976
> This is an Echo Bot ~~~
> f
> exit
The Peer connection: ws://localhost:57976 is closed
The Signal Server connection: ws://localhost:50864/ is closed
'''

'''
$ python test_signal.py passive
Connected to Signal Server: ws://localhost:50864/
Connected to Peer Server: ws://localhost:57976
> This is an Echo Bot ~~~
agent1
1
> f
> f
agent1
2
> exit
The Peer connection: ws://localhost:57976 is closed
The Signal Server connection: ws://localhost:50864/ is closed
Channel can only connect once, use another channel instead
True
'''


from gymize.channel import Channel

channel = Channel('kart')
channel.connect_sync()

while True:
    content, done = channel.wait_message('agent1')
    if done:
        break
    print(f'> {content}')
    channel.tell_sync('agent1', content)

# while channel.is_running():
#     channel.wait()
#     print('Hello')
#     data, _ = channel.wait_message('agent1')
#     print(f'> {data}')
#     channel.tell_sync('agent1', data)