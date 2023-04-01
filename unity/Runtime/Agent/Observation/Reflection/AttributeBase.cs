using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Gymize
{
    public abstract class AttributeBase : Attribute
    {
        List<string> m_Locations;
        public List<string> Locations
        {
            get { return m_Locations; }
            set { m_Locations = value; }
        }

        public AttributeBase()
        {
            m_Locations = new List<string>();
        }
        public AttributeBase(string location)
        {
            // location should not be null
            m_Locations = new List<string>{ location };
        }
        public AttributeBase(List<string> locations)
        {
            // locations should not be null
            m_Locations = locations;
        }

        public List<string> GetLocations()
        {
            return m_Locations;
        }

        public abstract IData GetData(object o);
    }
}