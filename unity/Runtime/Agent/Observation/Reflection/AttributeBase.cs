using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public abstract class AttributeBase : Attribute
    {
        List<string> m_Fields;
        public List<string> Fields
        {
            get { return m_Fields; }
            set { m_Fields = value; }
        }

        public AttributeBase()
        {
            m_Fields = new List<string>();
        }
        public AttributeBase(string field)
        {
            // field should not be null
            m_Fields = new List<string>{ field };
        }
        public AttributeBase(List<string> fields)
        {
            // fields should not be null
            m_Fields = fields;
        }

        public List<FieldString> GetFieldStrings()
        {
            return FieldString.ParseFrom(m_Fields);
        }

        public abstract IData GetData(object o);
    }
}