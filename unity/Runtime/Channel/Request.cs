using System.Threading.Tasks;
using Google.Protobuf;
using Gymize.Protobuf;

namespace Gymize
{
    public class Request
    {
        private string m_Id;
        public string Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        private byte[] m_Uuid;
        public byte[] Uuid
        {
            get { return m_Uuid; }
            set { m_Uuid = value; }
        }
        private Content m_Content;
        public Content Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }
        private Channel m_Channel;

        public Request(string id, byte[] uuid, Content content, Channel channel)
        {
            m_Id = id;
            m_Uuid = uuid;
            m_Content = content;
            m_Channel = channel;
        }

        public Request(string id, byte[] uuid, string text, Channel channel)
        {
            m_Id = id;
            m_Uuid = uuid;
            m_Content = new Content(text);
            m_Channel = channel;
        }

        public Request(string id, byte[] uuid, byte[] data, Channel channel)
        {
            m_Id = id;
            m_Uuid = uuid;
            m_Content = new Content(data);
            m_Channel = channel;
        }

        public async Task ReplyAsync(Content content)
        {
            MessageProto msg = new MessageProto
            {
                Header = new HeaderProto(),
                Content = new ContentProto()
            };
            msg.Header.MessageType = MessageTypeProto.Response;
            if (m_Id != null)
            {
                msg.Header.Id = m_Id;
            }
            msg.Header.Uuid = ByteString.CopyFrom(m_Uuid);
            if (content.IsText)
            {
                msg.Content.Text = content.Text;
            }
            if (content.IsBinary)
            {
                msg.Content.Raw = ByteString.CopyFrom(content.Raw);
            }
            await m_Channel.SendAsync(msg);
        }

        public async Task ReplyAsync(string text)
        {
            await ReplyAsync(new Content(text));
        }

        public async Task ReplyAsync(byte[] data)
        {
            await ReplyAsync(new Content(data));
        }

        public Task ReplySync(Content content)
        {
            return m_Channel.AddTask(() => ReplyAsync(content));
        }

        public Task ReplySync(string text)
        {
            return ReplySync(new Content(text));
        }

        public Task ReplySync(byte[] data)
        {
            return ReplySync(new Content(data));
        }
    }
}