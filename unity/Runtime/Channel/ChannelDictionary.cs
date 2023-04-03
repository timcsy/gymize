using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Gymize
{
    public class ChannelDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> m_Dictionary;
        public ChannelDictionary()
        {
            m_Dictionary = new Dictionary<TKey, TValue>();
        }
        public ChannelDictionary(Dictionary<TKey, TValue> dictionary)
        {
            m_Dictionary = dictionary;
        }

        public int Count
        {
            get { return m_Dictionary.Count; }
        }

        public bool IsReadOnly
        {
            // get { return m_Dictionary.IsReadOnly; }
            get { return false; }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!m_Dictionary.ContainsKey(key))
                {
                    TValue t = (TValue)Activator.CreateInstance(typeof(TValue));
                    m_Dictionary.Add(key, t);
                }
                return m_Dictionary[key];
            }
            set
            {
                if (!m_Dictionary.ContainsKey(key)) { m_Dictionary.Add(key, value); }
                else m_Dictionary[key] = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get { return m_Dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return m_Dictionary.Values; }
        }

        public void Add(KeyValuePair<TKey,TValue> item)
        {
            m_Dictionary.Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            m_Dictionary.Add(key, value);
        }

        public void Clear()
        {
            m_Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey,TValue> item)
        {
            if (m_Dictionary.ContainsKey(item.Key))
            {
                if (m_Dictionary[item.Key].Equals(item.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return m_Dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey,TValue>[] array, int arrayIndex)
        {
            // Not implemented
        }

        public IEnumerator GetEnumerator()
        {
            return m_Dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return m_Dictionary.GetEnumerator();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return m_Dictionary.Remove(item.Key);
        }

        public bool Remove(TKey key)
        {
            return m_Dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }
    }
}
