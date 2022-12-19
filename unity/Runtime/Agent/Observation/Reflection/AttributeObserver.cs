using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public abstract class AttributeObserver : IObserver
    {
        AttributeBase m_Attribute;

        public AttributeObserver(AttributeBase attr)
        {
            m_Attribute = attr;
        }

        public List<string> GetFields()
        {
            return m_Attribute.Fields;
        }

        public abstract IData GetData();
    }
}
