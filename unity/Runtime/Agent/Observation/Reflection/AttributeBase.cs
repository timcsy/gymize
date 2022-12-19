using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public abstract class AttributeBase : Attribute
    {
        List<string> m_Fields;
        string m_AgentName;

        public AttributeBase(List<string> fields = null, string agentName = null)
        {
            m_Fields = fields;
            m_AgentName = agentName;
        }
        public AttributeBase(string field = null, string agentName = null)
        {
            m_Fields = new List<string>();
            m_Fields.Add(field);
            m_AgentName = agentName;
        }

        public List<string> Fields
        {
            get { return m_Fields; }
            set { m_Fields = value; }
        }

        public string AgentName
        {
            get { return m_AgentName; }
            set { m_AgentName = value; }
        }
    }
}