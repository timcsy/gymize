using System;
using System.Collections;
using System.Collections.Generic;

namespace Gymize
{
    public abstract class AttributeBase : Attribute
    {
        private string m_Locator;

        public string Locator
        {
            get { return m_Locator; }
            set { m_Locator = value; }
        }

        public AttributeBase()
        {
            m_Locator = null;
        }
        public AttributeBase(string locator)
        {
            m_Locator = locator;
        }

        public string GetLocator()
        {
            return m_Locator;
        }

        public abstract IInstance GetData(object o);
    }
}