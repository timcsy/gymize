using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public class BoxAttribute : Attribute, IDataAttribute
    {
        string m_Name;
        string m_AgentName;

        public BoxAttribute(string name = null, string agentName = null)
        {
            m_Name = name;
            m_AgentName = agentName;
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
    }
}