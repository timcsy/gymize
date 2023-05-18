using System.Reflection;

namespace Gymize
{
    public class AttributeObserver : IObserver
    {
        AttributeBase m_Attribute;
        object m_Object;
        MemberInfo m_MemberInfo;

        public AttributeObserver(AttributeBase attr, object o, MemberInfo memberInfo)
        {
            m_Attribute = attr;
            m_Object = o;
            m_MemberInfo = memberInfo;
        }

        public string GetLocator()
        {
            return m_Attribute.Locator;
        }

        public IInstance GetObservation()
        {
            FieldInfo fieldInfo = m_MemberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                object o = fieldInfo.GetValue(m_Object);
                return m_Attribute.GetData(o);
            }
            PropertyInfo propInfo = m_MemberInfo as PropertyInfo;
            if (propInfo != null)
            {
                object o = propInfo.GetMethod.Invoke(m_Object, null);
                return m_Attribute.GetData(o);
            }
            return null;
        }
    }
}
