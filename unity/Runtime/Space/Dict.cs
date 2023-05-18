using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Gymize.Protobuf;

namespace Gymize
{
    public class Dict : IInstance, IDictionary<string, IInstance>
    {
        Dictionary<string, IInstance> m_Dict;

        public Dict()
        {
            m_Dict = new Dictionary<string, IInstance>();
        }
        public Dict(Dictionary<string, IInstance> dict)
        {
            m_Dict = new Dictionary<string, IInstance>(dict);
        }
        public Dict(IDictionary dict)
        {
            m_Dict = new Dictionary<string, IInstance>();
            foreach (DictionaryEntry entry in dict)
            {
                Add(entry);
            }
        }

        public int Count
        {
            get { return m_Dict.Count; }
        }

        public bool IsReadOnly
        {
            // get { return m_Dict.IsReadOnly; }
            get { return false; }
        }

        public IInstance this[string key]
        {
            get { return m_Dict[key]; }
            set { m_Dict[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return m_Dict.Keys; }
        }

        public ICollection<IInstance> Values
        {
            get { return m_Dict.Values; }
        }

        public void Add(KeyValuePair<string, IInstance> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(DictionaryEntry item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(string key, IInstance value)
        {
            m_Dict.Add(key, value);
        }

        public void Add(object key, object value)
        {
            Add((string)key, GymInstance.ToGym(value));
        }

        public void Clear()
        {
            m_Dict.Clear();
        }

        public bool Contains(KeyValuePair<string, IInstance> item)
        {
            if (m_Dict.ContainsKey(item.Key))
            {
                if (m_Dict[item.Key].Equals(item.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsKey(string key)
        {
            return m_Dict.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, IInstance>[] array, int arrayIndex)
        {
            throw new NotImplementedException("");
        }

        public IEnumerator GetEnumerator()
        {
            return m_Dict.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, IInstance>> IEnumerable<KeyValuePair<string, IInstance>>.GetEnumerator() {
            return m_Dict.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, IInstance> item)
        {
            return m_Dict.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            return m_Dict.Remove(key);
        }

        public bool TryGetValue(string key, out IInstance value)
        {
            return m_Dict.TryGetValue(key, out value);
        }

        public InstanceProto ToProtobuf()
        {
            InstanceProto instance = new InstanceProto();
            instance.Type = InstanceTypeProto.Dict;
            foreach (KeyValuePair<string, IInstance> pair in m_Dict)
            {
                string key = pair.Key;
                IInstance value = pair.Value;
                instance.Dict[key] = value.ToProtobuf();
            }
            return instance;
        }

        public static object ParseFrom(MapField<string, InstanceProto> dictProto)
        {
            // Convert to Dictionary<string, object>
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (KeyValuePair<string, InstanceProto> pair in dictProto)
            {
                string key = pair.Key;
                InstanceProto proto = pair.Value;
                dict[key] = GymInstance.ParseFrom(proto);
            }
            return dict;
        }

        public static implicit operator Dictionary<string, IInstance>(Dict dict) => dict.m_Dict;

        public override string ToString()
        {
            string output = "{";
            int i = 0;
            foreach (KeyValuePair<string, IInstance> pair in m_Dict)
            {
                output += pair.Key + ": " + pair.Value.ToString();
                if (i < m_Dict.Count - 1) output += ", ";
                i++;
            }
            output += "}";
            return output;
        }
    }
}