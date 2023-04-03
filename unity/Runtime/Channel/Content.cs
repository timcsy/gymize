namespace PAIA.Gymize
{
    public class Content
    {
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }
        public byte[] Raw
        {
            get { return m_Raw; }
            set { m_Raw = value; }
        }
        private string m_Text = null;
        private byte[] m_Raw = null;

        public Content(string text)
        {
            m_Text = text;
        }
        public Content(byte[] data)
        {
            m_Raw = data;
        }

        public bool IsText
        {
            get { return m_Text != null; }
        }
        public bool IsBinary
        {
            get { return m_Raw != null; }
        }

        public override string ToString()
        {
            if (IsText) return m_Text;
            else if (IsBinary) return m_Raw.ToString();
            else return null;
        }
    }
}