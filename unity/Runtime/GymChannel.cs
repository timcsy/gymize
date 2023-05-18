using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Gymize
{
    public class GymChannel
    {
        Channel m_Channel;
        public DelegateDictionary<string, MessageCallBack> OnMessage; // { id: MessageCallBack }
        public DelegateDictionary<string, RequestCallBack> OnRequest; // { id: RequestCallBack }

        public GymChannel()
        {
            m_Channel = null;
            OnMessage = new DelegateDictionary<string, MessageCallBack>
            {
                { "", null }
            };
            OnRequest = new DelegateDictionary<string, RequestCallBack>
            {
                { "", null }
            };
        }

        public async Task TellAsync(string id, Content content)
        {
            await m_Channel?.TellAsync(id, content);
        }

        public async Task TellAsync(string id, string text)
        {
            await m_Channel?.TellAsync(id, text);
        }

        public async Task TellAsync(string id, byte[] data)
        {
            await m_Channel?.TellAsync(id, data);
        }

        public Task TellSync(string id, Content content)
        {
            return m_Channel?.TellSync(id, content);
        }

        public Task TellSync(string id, string text)
        {
            return m_Channel?.TellSync(id, text);
        }

        public Task TellSync(string id, byte[] data)
        {
            return m_Channel?.TellSync(id, data);
        }

        public async Task BroadcastAsync(Content content)
        {
            await m_Channel?.BroadcastAsync(content);
        }

        public async Task BroadcastAsync(string text)
        {
            await m_Channel?.BroadcastAsync(text);
        }

        public async Task BroadcastAsync(byte[] data)
        {
            await m_Channel?.BroadcastAsync(data);
        }

        public Task BroadcastSync(Content content)
        {
            return m_Channel?.BroadcastSync(content);
        }

        public Task BroadcastSync(string text)
        {
            return m_Channel?.BroadcastSync(text);
        }

        public Task BroadcastSync(byte[] data)
        {
            return m_Channel?.BroadcastSync(data);
        }

        public async Task<Content> AskAsync(string id, Content content)
        {
            return await m_Channel?.AskAsync(id, content);
        }

        public async Task<Content> AskAsync(string id, string text)
        {
            return await m_Channel?.AskAsync(id, text);
        }

        public async Task<Content> AskAsync(string id, byte[] data)
        {
            return await m_Channel?.AskAsync(id, data);
        }

        public Task<Content> AskSync(string id, Content content)
        {
            return m_Channel?.AskSync(id, content);
        }

        public Task<Content> AskSync(string id, string text)
        {
            return m_Channel?.AskSync(id, text);
        }

        public Task<Content> AskSync(string id, byte[] data)
        {
            return m_Channel?.AskSync(id, data);
        }

        // check if receive message, response
        // :return: done
        public bool HasRecv()
        {
            if (m_Channel == null) return false;
            return m_Channel.HasRecv();
        }

        // wait until receive message, response or channel closed
        // :return: done
        public IEnumerator Wait(float pollingSecs = 0.001f)
        {
            yield return m_Channel?.Wait(pollingSecs);
        }

        // check if receive message
        // :return: done
        public bool HasMessage(string id)
        {
            if (m_Channel == null) return false;
            return m_Channel.HasMessage(id);
        }

        // take message and pop from the queue
        // :return: data
        public Content TakeMessage(string id)
        {
            return m_Channel?.TakeMessage(id);
        }

        // after received a message, or the channel is closed
        // it will return data, if data is null, means the channel is closed
        // :return: data
        public IEnumerator WaitMessage(string id, float pollingSecs = 0.001f)
        {
            yield return m_Channel?.WaitMessage(id, pollingSecs);
        }

        // take response and remove
        // :return: data
        public Content TakeResponse(Task<Content> response)
        {
            return m_Channel?.TakeResponse(response);
        }

        // after received a response, or the channel is closed
        // it will return data, if data is null, means the channel is closed
        // :return: data
        public IEnumerator WaitResponse(Task<Content> response, float pollingSecs = 0.001f)
        {
            return m_Channel?.WaitResponse(response, pollingSecs);
        }

        internal void StartChannel(string name)
        {
            m_Channel = new Channel(name);
            m_Channel.OnMessage = OnMessage;
            m_Channel.OnRequest = OnRequest;
            m_Channel.ConnectSync();
        }

        internal void CloseChannel()
        {
            if (m_Channel != null)
            {
                m_Channel.CloseSync();
                m_Channel = null;
            }
        }

        internal void QuitWhenChannelClosed()
        {
            if (m_Channel?.Status == ChannelStatus.CLOSED)
            {
                #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }
    }
}