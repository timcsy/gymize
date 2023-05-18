using UnityEngine;
using Gymize.Protobuf;

namespace Gymize
{
    public class Json : IInstance
    {
        private object m_Object;
        public object Object
        {
            get { return m_Object; }
            set { m_Object = value; }
        }

        public Json()
        {
            m_Object = null;
        }
        public Json(object obj)
        {
            m_Object = obj;
        }

        public InstanceProto ToProtobuf()
        {
            InstanceProto instance = new InstanceProto
            {
                Type = InstanceTypeProto.Json,
                Json = ToString()
            };
            return instance;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(m_Object);
        }
    }
}