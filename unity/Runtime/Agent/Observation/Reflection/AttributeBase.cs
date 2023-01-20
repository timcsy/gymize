using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public abstract class AttributeBase : Attribute
    {
        List<string> m_Fields;

        public AttributeBase()
        {
            m_Fields = new List<string>();
            m_Fields.Add("");
        }
        public AttributeBase(string field)
        {
            m_Fields = new List<string>();
            if (field != null) m_Fields.Add(field);
        }
        public AttributeBase(List<string> fields)
        {
            if (fields == null) m_Fields = new List<string>();
            else m_Fields = fields;
        }

        public List<string> Fields
        {
            get { return m_Fields; }
            set { m_Fields = value; }
        }
    }
}