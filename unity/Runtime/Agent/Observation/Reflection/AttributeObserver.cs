using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PAIA.Gymize
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

        public IData GetObservation(int cacheId = -1)
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
