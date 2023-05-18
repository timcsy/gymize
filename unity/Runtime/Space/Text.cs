using Gymize.Protobuf;

namespace Gymize
{
    public class Text : IInstance
    {
        private string m_Text;
        public string Value
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public Text()
        {
            m_Text = null;
        }
        public Text(string text)
        {
            m_Text = text;
        }
        public Text(object obj)
        {
            m_Text = (string)obj;
        }

        public InstanceProto ToProtobuf()
        {
            InstanceProto instance = new InstanceProto
            {
                Type = InstanceTypeProto.Text,
                Text = m_Text
            };
            return instance;
        }

        public static object ParseFrom(string text)
        {
            // Convert to string
            return text;
        }

        public override string ToString()
        {
            return m_Text;
        }
    }
}