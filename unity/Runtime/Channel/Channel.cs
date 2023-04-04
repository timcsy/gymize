using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using Google.Protobuf;
using PAIA.Gymize.Protobuf;

namespace PAIA.Gymize
{
    public enum ChannelMode
    {
        ACTIVE,
        PASSIVE
    }

    public enum ChannelStatus
    {
        NONE,
        CONNECTING,
        CONNECTED,
        DISCONNECTED,
        CLOSED
    }

    public class Channel : IDisposable
    {
        // Public variables
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public ChannelMode Mode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }
        public ChannelStatus Status
        {
            get { return m_Status; }
        }

        // Private Varaibles
        private bool m_Disposed;

        // For Signal Server
        private string m_Name; // game name (e.g. kart)
        private ChannelMode m_Mode;
        private string m_SignalingUrl;
        private string m_Protocol;
        private string m_Host;
        private int m_Port;
        private bool m_UsingAvailablePort;
        internal string m_PeerUrl;
        private bool m_UsingExistingUrl;
        private string m_SignalingId;
        private WebSocket m_WsSignaling;

        // For channel
        internal ChannelStatus m_Status; // CONNECTING, CONNECTED, DISCONNECTED, CLOSED
        internal bool m_Retry; // whether to retry connection when disconnected
        internal WebSocketServer m_WsPeerServer; // Peer websocket server
        internal WsServerRecv m_WsPeerBehavior; // Peer websocket server route
        private WebSocket m_WsPeerClient; // Peer websocket client
        private CancellationTokenSource m_ChannelStop; // if the channel is stopped
        internal bool m_Updating; // if the channel peer server is updating
        private bool m_Sending; // if the channel peer server is sending messages
        private Queue<Message> m_Outbox; // Queue[Message]
        private Dictionary<string, Queue<Content>> m_Inbox; // { id: Queue[Content] }
        private Dictionary<ByteString, TaskCompletionSource<Content>> m_Responses; // { uuid: Future[Content] }
        // event_name: open, error, signaling_disconnected, peer_disconnected, close
        
        public event CallBack OnOpen;
        public event ErrorCallBack OnError;
        public event CallBack OnSignalingDisconnected;
        public event CallBack OnPeerDisconnected;
        public event CallBack OnClose;
        public ChannelDictionary<string, ObjectCallBackEvent> On; // { event_name: ObjectCallBackEvent }
        public ChannelDictionary<string, MessageCallBackEvent> OnMessage; // { id: MessageCallBackEvent }
        public ChannelDictionary<string, RequestCallBackEvent> OnRequest; // { id: RequestCallBackEvent }

        public Channel(string name, ChannelMode mode = ChannelMode.PASSIVE, string signalingUrl = "ws://localhost:50864/", string protocol = "ws", string host = "localhost", int port = -1, string peer_url = null, bool retry = true)
        {
            m_Disposed = false;

            m_Name = name;
            m_Mode = mode;
            m_SignalingUrl = signalingUrl;
            m_Protocol = protocol;
            m_Host = host;
            m_Port = port;
            m_UsingAvailablePort = port < 0;
            m_PeerUrl = peer_url;
            m_UsingExistingUrl = peer_url != null;
            m_SignalingId = null;
            m_WsSignaling = null;

            m_Status = ChannelStatus.NONE;
            m_Retry = retry;
            m_WsPeerServer = null;
            m_WsPeerBehavior = null;
            m_WsPeerClient = null;
            m_ChannelStop = new CancellationTokenSource();
            m_Updating = false;
            m_Sending = false;
            m_Outbox = new Queue<Message>();
            m_Inbox = new Dictionary<string, Queue<Content>>()
            {
                { "", new Queue<Content>() }
            };
            m_Responses = new Dictionary<ByteString, TaskCompletionSource<Content>>();
            On = new ChannelDictionary<string, ObjectCallBackEvent>
            (
                new Dictionary<string, ObjectCallBackEvent>()
                {
                }
            );
            OnMessage = new ChannelDictionary<string, MessageCallBackEvent>
            (
                new Dictionary<string, MessageCallBackEvent>()
                {
                    { "", new MessageCallBackEvent() }
                }
            );
            OnRequest = new ChannelDictionary<string, RequestCallBackEvent>
            (
                new Dictionary<string, RequestCallBackEvent>()
                {
                    { "", new RequestCallBackEvent() }
                }
            );
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(disposing: true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!m_Disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    m_ChannelStop.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                CloseSync();

                // Note disposing has been done.
                m_Disposed = true;
            }
        }

        ~Channel()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(disposing: false) is optimal in terms of
            // readability and maintainability.
            Dispose(disposing: false);
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //                                   Async part                                    //
        /////////////////////////////////////////////////////////////////////////////////////

        public bool IsRunning()
        {
            return !m_ChannelStop.IsCancellationRequested;
        }

        public IEnumerator WaitFinishSync(float pollingSecs = 0.001f)
        {
            while (IsRunning())
            {
                yield return new WaitForSeconds(pollingSecs);
            }
        }

        public Task AddTask(Action action)
        {
            try
            {
                return Task.Run(action, m_ChannelStop.Token);
            }
            catch
            {
                return null;
            }
        }

        public Task AddTask(Func<System.Threading.Tasks.Task> function)
        {
            try
            {
                return Task.Run(function, m_ChannelStop.Token);
            }
            catch
            {
                return null;
            }
        }

        public Task<Content> AddTask(Func<System.Threading.Tasks.Task<Content>> function)
        {
            try
            {
                return Task.Run<Content>(function, m_ChannelStop.Token);
            }
            catch
            {
                return null;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //                              Channel function part                              //
        /////////////////////////////////////////////////////////////////////////////////////

        public async Task ConnectAsync()
        {
            if (m_Status == ChannelStatus.NONE)
            {
                Signaling();
                await Task.CompletedTask;
            }
            else
            {
                Debug.Log("Channel can only connect once, use another channel instead");
            }
        }

        public Task ConnectSync()
        {
            return AddTask(() => ConnectAsync());
        }

        public async Task TellAsync(string id, Content content)
        {
            Protobuf.Message msg = new Protobuf.Message
            {
                Header = new Protobuf.Header(),
                Content = new Protobuf.Content()
            };
            msg.Header.MessageType = Protobuf.MessageType.Message;
            if (id != null)
            {
                msg.Header.Id = id;
            }
            if (content.IsText)
            {
                msg.Content.Text = content.Text;
            }
            if (content.IsBinary)
            {
                msg.Content.Raw = ByteString.CopyFrom(content.Raw);
            }
            await SendAsync(msg);
        }

        public async Task TellAsync(string id, string text)
        {
            await TellAsync(id, new Content(text));
        }

        public async Task TellAsync(string id, byte[] data)
        {
            await TellAsync(id, new Content(data));
        }

        public Task TellSync(string id, Content content)
        {
            return AddTask(() => TellAsync(id, content));
        }

        public Task TellSync(string id, string text)
        {
            return TellSync(id, new Content(text));
        }

        public Task TellSync(string id, byte[] data)
        {
            return TellSync(id, new Content(data));
        }

        public async Task BroadcastAsync(Content content)
        {
            await TellAsync(null, content);
        }

        public async Task BroadcastAsync(string text)
        {
            await BroadcastAsync(new Content(text));
        }

        public async Task BroadcastAsync(byte[] data)
        {
            await BroadcastAsync(new Content(data));
        }

        public Task BroadcastSync(Content content)
        {
            return AddTask(() => BroadcastAsync(content));
        }

        public Task BroadcastSync(string text)
        {
            return BroadcastSync(new Content(text));
        }

        public Task BroadcastSync(byte[] data)
        {
            return BroadcastSync(new Content(data));
        }

        public async Task<Content> AskAsync(string id, Content content)
        {
            if (!m_Inbox.ContainsKey(id))
            {
                m_Inbox.Add(id, new Queue<Content>());
            }

            Protobuf.Message msg = new Protobuf.Message
            {
                Header = new Protobuf.Header(),
                Content = new Protobuf.Content()
            };
            msg.Header.MessageType = Protobuf.MessageType.Request;
            if (id != null)
            {
                msg.Header.Id = id;
            }
            byte[] uuid = Guid.NewGuid().ToByteArray();
            msg.Header.Uuid = ByteString.CopyFrom(uuid);
            if (content.IsText)
            {
                msg.Content.Text = content.Text;
            }
            if (content.IsBinary)
            {
                msg.Content.Raw = ByteString.CopyFrom(content.Raw);
            }
            m_Responses[msg.Header.Uuid] = new TaskCompletionSource<Content>();
            await SendAsync(msg);
            Content response = await m_Responses[msg.Header.Uuid].Task;
            m_Responses.Remove(msg.Header.Uuid);
            return response;
        }

        public async Task<Content> AskAsync(string id, string text)
        {
            return await AskAsync(id, new Content(text));
        }

        public async Task<Content> AskAsync(string id, byte[] data)
        {
            return await AskAsync(id, new Content(data));
        }

        public Task<Content> AskSync(string id, Content content)
        {
            return AddTask(() => AskAsync(id, content));
        }

        public Task<Content> AskSync(string id, string text)
        {
            return AskSync(id, new Content(text));
        }

        public Task<Content> AskSync(string id, byte[] data)
        {
            return AskSync(id, new Content(data));
        }

        public async Task SendAsync(Message msg)
        {
            m_Outbox.Enqueue(msg);
            SendFlush(); // flush the outbox because the outbox is not empty now
            await Task.CompletedTask;
        }

        public Task SendSync(Message msg)
        {
            return AddTask(() => SendAsync(msg));
        }

        internal void SendFlush()
        {
            if (m_Sending) { return; }
            m_Sending = true;
            while (m_Outbox.Count > 0)
            {
                if (m_WsPeerClient != null && m_WsPeerClient.IsAlive)
                {
                    Protobuf.Message msg = m_Outbox.Dequeue();
                    m_WsPeerClient.Send(msg?.ToByteArray());
                }
                else if (m_WsPeerBehavior != null)
                {
                    Protobuf.Message msg = m_Outbox.Dequeue();
                    m_WsPeerBehavior.SendSync(msg?.ToByteArray());
                }
                else { break; }
            }
            m_Sending = false;
        }

        internal void Recv(byte[] data)
        {
            Protobuf.Message msg = Protobuf.Message.Parser.ParseFrom(data);
            Content content = null;
            switch (msg.Content.DataCase)
            {
                case Protobuf.Content.DataOneofCase.Raw:
                    content = new Content(msg.Content.Raw.ToByteArray());
                    break;
                case Protobuf.Content.DataOneofCase.Text:
                    content = new Content(msg.Content.Text);
                    break;
                default:
                    break;
            }

            if (msg.Header.MessageType == Protobuf.MessageType.Message)
            {
                if (msg.Header.Id != "")
                {
                    if (!m_Inbox.ContainsKey(msg.Header.Id))
                    {
                        m_Inbox.Add(msg.Header.Id, new Queue<Content>());
                    }

                    // only put the message to the queue if there is no message listener
                    if (OnMessage[msg.Header.Id].Empty)
                    {
                        m_Inbox[msg.Header.Id].Enqueue(content);
                    }
                    else
                    {
                        TriggerMessage(msg.Header.Id, content);
                    }
                }
                else
                {
                    // id == "" means broadcast, and "" means root channel itself
                    foreach (string id in m_Inbox.Keys)
                    {
                        // only put the message to the queue if there is no message listener
                        if (OnMessage[id].Empty)
                        {
                            m_Inbox[id].Enqueue(content);
                        }
                        else
                        {
                            TriggerMessage(id, content);
                        }
                    }
                }
            }
            else if (msg.Header.MessageType == Protobuf.MessageType.Request)
            {
                byte[] uuid = msg.Header.Uuid.ToByteArray();
                Request request = new Request(msg.Header.Id, uuid, content, this);
                if (msg.Header.Id != "")
                {
                    TriggerRequest(msg.Header.Id, request);
                }
                else
                {
                    Trigger("request", request);
                }
            }
            else if (msg.Header.MessageType == Protobuf.MessageType.Response)
            {
                byte[] uuid = msg.Header.Uuid.ToByteArray();
                m_Responses[msg.Header.Uuid].TrySetResult(content);
            }
        }

        // check if receive message, response
        // :return: done
        public bool HasRecv()
        {
            foreach (string id in m_Inbox.Keys)
            {
                if (m_Inbox[id].Count > 0) { return true; }
            }
            foreach (TaskCompletionSource<Content> tcs in m_Responses.Values)
            {
                if (tcs.Task.IsCompleted) { return true; }
            }
            return false;
        }

        // wait until receive message, response or channel closed
        // :return: done
        public IEnumerator Wait(float pollingSecs = 0.001f)
        {
            while (!HasRecv() && IsRunning())
            {
                yield return new WaitForSeconds(pollingSecs);
            }
            yield return !IsRunning();
        }

        // check if receive message
        // :return: done
        public bool HasMessage(string id)
        {
            if (!m_Inbox.ContainsKey(id))
            {
                m_Inbox.Add(id, new Queue<Content>());
            }
            return m_Inbox[id].Count > 0;
        }

        // take message and pop from the queue
        // :return: data
        public Content TakeMessage(string id)
        {
            Content data = null;
            if (HasMessage(id))
            {
                data = m_Inbox[id].Dequeue();
            }
            return data;
        }

        // after received a message, or the channel is closed
        // it will return data, if data is null, means the channel is closed
        // :return: data
        public IEnumerator WaitMessage(string id, float pollingSecs = 0.001f)
        {
            while (!HasMessage(id) && IsRunning())
            {
                yield return new WaitForSeconds(pollingSecs);
            }
            yield return TakeMessage(id);
        }

        // take response and remove
        // :return: data
        public Content TakeResponse(Task<Content> response)
        {
            Content data = null;
            if (response.IsCompleted)
            {
                data = response.Result;
            }
            return data;
        }

        // after received a response, or the channel is closed
        // it will return data, if data is null, means the channel is closed
        // :return: data
        public IEnumerator WaitResponse(Task<Content> response, float pollingSecs = 0.001f)
        {
            while (!response.IsCompleted && IsRunning())
            {
                yield return new WaitForSeconds(pollingSecs);
            }
            yield return TakeResponse(response);
        }

        public async Task PauseAsync()
        {
            if (m_WsSignaling != null && m_WsSignaling.IsAlive)
            {
                m_WsSignaling.Close(CloseStatusCode.Normal);
                await Task.CompletedTask;
            }
        }

        public Task PauseSync()
        {
            return AddTask(() => PauseAsync());
        }
        
        // resume the signal connection with current id
        public void ResumeAsync()
        {
            Signaling(true);
        }

        public Task ResumeSync()
        {
            return AddTask(() => ResumeAsync());
        }

        public async Task CloseAsync()
        {
            CloseSync();
            await Task.CompletedTask;
        }

        public void CloseSync()
        {
            try
            {
                m_Status = ChannelStatus.CLOSED;
                if (m_WsSignaling!= null && m_WsSignaling.IsAlive)
                {
                    Protobuf.Signal signal = new Protobuf.Signal();
                    signal.SignalType = Protobuf.SignalType.Close;
                    signal.Id = m_SignalingId;
                    m_WsSignaling.Send(signal?.ToByteArray());
                }
                if (m_WsPeerClient!= null && m_WsPeerClient.IsAlive)
                {
                    m_WsPeerClient.Close(CloseStatusCode.Normal);
                }
                m_WsPeerClient = null;
                if (m_WsPeerServer != null)
                {
                    m_WsPeerServer.Stop();
                }
                m_WsPeerBehavior = null;
            }
            catch {}
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //                             Event-driven style part                             //
        /////////////////////////////////////////////////////////////////////////////////////

        // Event names: open, error, signaling_disconnected, peer_disconnected, close

        // Add Event Listeners:
        // channel.OnOpen += () => { };
        // channel.OnError += (e) => { };
        // channel.OnSignalingDisconnected += () => { };
        // channel.OnPeerDisconnected += () => { };
        // channel.OnClose += () => { };
        // channel.On["event_name"].Event += (e) => { };
        // channel.OnMessage["id"].Event += (content) => { };
        // channel.OnResponse["id"].Event += (request) => { };

        public void Trigger(string eventName, object e = null)
        {
            if (eventName == "message")
            {
                Content content = e as Content;
                if (content != null) TriggerMessage(id: "", content: content);
            }
            else if (eventName == "request")
            {
                Request request = e as Request;
                if (request != null) TriggerRequest(id: "", request: request);
            }
            else if (eventName == "open")
            {
                OnOpen?.Invoke();
            }
            else if (eventName == "error")
            {
                OnError?.Invoke(e as ErrorEventArgs);
            }
            else if (eventName == "signaling_disconnected")
            {
                OnSignalingDisconnected?.Invoke();
            }
            else if (eventName == "peer_disconnected")
            {
                OnPeerDisconnected?.Invoke();
            }
            else if (eventName == "close")
            {
                OnClose?.Invoke();
            }
            else
            {
                On[eventName].Invoke(e);
            }
        }

        public void TriggerMessage(string id, Content content)
        {
            OnMessage[id].Invoke(content);
        }

        public void TriggerRequest(string id, Request request)
        {
            OnRequest[id].Invoke(request);
        }

        // Remove Event Listeners:
        // channel.OnOpen -= event_handler
        // channel.OnError -= event_handler
        // channel.OnSignalingDisconnected -= event_handler
        // channel.OnPeerDisconnected -= event_handler
        // channel.OnClose -= event_handler
        // channel.On["event_name"].Event -= event_handler
        // channel.OnMessage["id"].Event -= event_handler
        // channel.OnResponse["id"].Event -= event_handler

        /////////////////////////////////////////////////////////////////////////////////////
        //                               Signal client  part                               //
        /////////////////////////////////////////////////////////////////////////////////////

        // Note that the Unity side will not create a signal server!
        private void EnsureSignalingServerOpened()
        {
            WebSocket ws = new WebSocket(m_SignalingUrl);
            ws.Connect();
            if (ws.ReadyState != WebSocketState.Open)
            {
                // The websocket url is unavailable
                m_SignalingUrl = "ws://localhost:50864"; // Using the local default signaling server created by Python
            }
            else
            {
                ws.Close(CloseStatusCode.Normal);
            }
        }
        
        private void Signaling(bool isResume = false)
        {
            m_Status = ChannelStatus.CONNECTING;

            EnsureSignalingServerOpened();

            m_WsSignaling = new WebSocket(m_SignalingUrl);
            m_WsSignaling.Compression = CompressionMethod.Deflate;
            m_WsSignaling.OnOpen += (sender, e) =>
            {
                Debug.Log("Connected to Signal Server: " + m_SignalingUrl);
                SignalingStart(isResume);
            };
            m_WsSignaling.OnMessage += async (sender, e) =>
            {
                if (e.IsBinary)
                {
                    await SignalingRecv(e.RawData);
                }
            };
            m_WsSignaling.OnError += (sender, e) =>
            {
                Trigger("error", e); // Note: e.Message is the error message
            };
            m_WsSignaling.OnClose += (sender, e) =>
            {
                m_WsSignaling = null;
                Debug.Log("The Signal Server connection: " + m_SignalingUrl + " is closed");
                if (m_Status == ChannelStatus.CLOSED)
                {
                    // close the channel
                    Trigger("close");
                    m_ChannelStop.Cancel();
                }
                else
                {
                    m_Status = ChannelStatus.DISCONNECTED;
                    Trigger("signaling_disconnected");
                    if (m_Retry)
                    {
                        ResumeAsync();
                    }
                }
            };
            m_WsSignaling.Connect();
        }

        private void SignalingStart(bool isResume = false)
        {
            Protobuf.Signal signal = new Protobuf.Signal();

            if (!isResume)
            {
                // initialize
                signal.SignalType = Protobuf.SignalType.Init;
                signal.Name = m_Name;
            }
            else
            {
                // resume
                signal.SignalType = Protobuf.SignalType.Resume;
                signal.Id = m_SignalingId;
            }

            if (m_Mode == ChannelMode.ACTIVE)
            {
                signal.PeerType = Protobuf.PeerType.Active;
            }
            else if (m_Mode == ChannelMode.PASSIVE)
            {
                signal.PeerType = Protobuf.PeerType.Passive;
            }
            if (m_WsSignaling != null && m_WsSignaling.IsAlive)
            {
                m_WsSignaling.Send(signal?.ToByteArray());
            }
        }

        private async Task SignalingRecv(byte[] msg)
        {
            Protobuf.Signal signal = Protobuf.Signal.Parser.ParseFrom(msg);

            // switch by signal type
            switch (signal.SignalType)
            {
                case Protobuf.SignalType.Init:
                    // set signaling id given by the signal server
                    m_SignalingId = signal.Id;
                    if (m_Mode == ChannelMode.ACTIVE)
                    {
                        await UpdateAsync(); // get a Peer Server
                    }
                    break;
                case Protobuf.SignalType.Update:
                    if (m_Mode == ChannelMode.ACTIVE)
                    {
                        await UpdateAsync(); // get a Peer Server
                    }
                    else if (m_Mode == ChannelMode.PASSIVE)
                    {
                        // connect to Peer Server, in another task
                        WsClient(signal.Url);
                    }
                    break;
                case Protobuf.SignalType.Close:
                    m_Status = ChannelStatus.CLOSED;
                    if (m_WsSignaling != null && m_WsSignaling.IsAlive)
                    {
                        m_WsSignaling.Close(CloseStatusCode.Normal);
                    }
                    m_WsSignaling = null;
                    break;
                default:
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////
        //                                Peer channel part                                //
        /////////////////////////////////////////////////////////////////////////////////////

        // Establish a new peer connection and replace the old one
        public async Task UpdateAsync(float waitingSecs = 1.0f)
        {
            // Lock when updating
            if (m_Updating) { return; }
            else
            {
                m_Updating = true;
                if (m_WsPeerClient != null && m_WsPeerClient.IsAlive)
                {
                    m_WsPeerClient.Close(CloseStatusCode.Normal);
                }
                m_WsPeerClient = null;
                if (m_WsPeerServer != null)
                {
                    m_WsPeerServer.Stop();
                }
                m_WsPeerBehavior = null;
            }

            Protobuf.Signal signal = new Protobuf.Signal();
            signal.SignalType = Protobuf.SignalType.Update;
            signal.Id = m_SignalingId;

            if (m_Mode == ChannelMode.ACTIVE)
            {
                // get Peer Server
                if (!m_UsingExistingUrl)
                {
                    m_PeerUrl = await CreatePeerServer(waitingSecs);
                }
                signal.Url = m_PeerUrl; // send url (protocol://host:port) to the other peer
            }
            else if (m_Mode == ChannelMode.PASSIVE)
            {
                // Ask active peer to raise an update signal
            }

            // send update information to the other peer
            if (m_WsSignaling!= null && m_WsSignaling.IsAlive)
            {
                m_WsSignaling.Send(signal?.ToByteArray());
            }
        }

        public Task UpdateSync(float waitingSecs = 1.0f)
        {
            return AddTask(() => UpdateAsync(waitingSecs));
        }

        private async Task<string> CreatePeerServer(float waitingSecs = 1.0f)
        {
            // Choose an available port by the system
            if (m_UsingAvailablePort)
            {
                TcpListener l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                int port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
                m_Port = port;
            }
            // start a websocket server in another task
            m_PeerUrl = $"{m_Protocol}://{m_Host}:{m_Port}";
            WsServer(m_PeerUrl);
            await Task.Delay((int)(waitingSecs * 1000));
            return m_PeerUrl;
        }

        private void WsServer(string url)
        {
            m_WsPeerServer = new WebSocketServer(url);
            m_WsPeerServer.AddWebSocketService<WsServerRecv> ("/", s => s.Channel = this);
            m_WsPeerServer.Start();
            Debug.Log("Start Peer Server: " + url);
        }

        private void WsClient(string url)
        {
            m_WsPeerClient = new WebSocket(url);
            m_WsPeerClient.Compression = CompressionMethod.Deflate;
            m_WsPeerClient.OnOpen += (sender, e) =>
            {
                m_PeerUrl = url;
                Debug.Log("Connected to Peer Server: " + m_PeerUrl);
                m_Updating = false;

                m_Status = ChannelStatus.CONNECTED;
                SendFlush(); // flush the outbox after connection is connected
                Trigger("open");
            };
            m_WsPeerClient.OnMessage += (sender, e) =>
            {
                if (e.IsBinary)
                {
                    Recv(e.RawData);
                }
            };
            m_WsPeerClient.OnError += (sender, e) =>
            {
                Trigger("error", e); // Note: e.Message is the error message
            };
            m_WsPeerClient.OnClose += async (sender, e) =>
            {
                m_WsPeerClient = null;
                Debug.Log("The Peer connection: " + m_SignalingUrl + " is closed");
                if (m_Status != ChannelStatus.CLOSED)
                {
                    m_Status = ChannelStatus.DISCONNECTED;
                    Trigger("peer_disconnected");
                    if (m_Retry)
                    {
                        await UpdateAsync();
                    }
                }
            };
            m_WsPeerClient.Connect();
        }
	}

    public class WsServerRecv : WebSocketBehavior
    {
        private Channel m_Channel = null;
        public Channel Channel
        {
            get { return m_Channel; }
            set { m_Channel = value; }
        }

        public void SendSync(byte[] data)
        {
            Send(data);
        }

        protected override void OnOpen()
        {
            m_Channel.m_WsPeerBehavior = this;
            m_Channel.m_Updating = false;
            m_Channel.m_Status = ChannelStatus.CONNECTED;
            m_Channel.SendFlush(); // flush the outbox after connection is connected
            m_Channel.Trigger("open");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                m_Channel.Recv(e.RawData);
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            m_Channel.Trigger("error", e); // Note: e.Message is the error message
        }

        protected override void OnClose(CloseEventArgs e)
        {
            m_Channel.m_WsPeerBehavior = null;
            m_Channel.m_WsPeerServer.Stop();
            m_Channel.m_WsPeerServer = null;
            Debug.Log("The Peer connection: " + m_Channel.m_PeerUrl + " is closed");
            if (m_Channel.m_Status != ChannelStatus.CLOSED)
            {
                m_Channel.m_Status = ChannelStatus.DISCONNECTED;
                m_Channel.Trigger("peer_disconnected");
                if (m_Channel.m_Retry)
                {
                    m_Channel.UpdateSync();
                }
            }
        }
    }
}
