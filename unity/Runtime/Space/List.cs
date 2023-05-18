using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Gymize.Protobuf;

namespace Gymize
{
    public class List : IInstance, IList<IInstance>
    {
        List<IInstance> m_List;

        public List()
        {
            m_List = new List<IInstance>();
        }
        public List(List<IInstance> list)
        {
            m_List = list;
        }
        public List(IEnumerable list)
        {
            m_List = new List<IInstance>();
            foreach (object item in list)
            {
                Add(item);
            }
        }

        public int Count
        {
            get { return m_List.Count; }
        }

        public bool IsReadOnly
        {
            // get { return m_List.IsReadOnly; }
            get { return false; }
        }

        public IInstance this[int index]
        {
            get { return m_List[index]; }
            set { m_List[index] = value; }
        }

        public void Add(IInstance item)
        {
            m_List.Add(item);
        }

        public void Add(object item)
        {
            m_List.Add(GymInstance.ToGym(item));
        }

        public void Clear()
        {
            m_List.Clear();
        }

        public bool Contains(IInstance item)
        {
            return m_List.Contains(item);
        }

        public void CopyTo(IInstance[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public IEnumerator GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator<IInstance> IEnumerable<IInstance>.GetEnumerator() {
            return m_List.GetEnumerator();
        }

        public int IndexOf(IInstance item)
        {
            return m_List.IndexOf(item);
        }

        public void Insert(int index, IInstance item)
        {
            m_List.Insert(index, item);
        }

        public bool Remove(IInstance item)
        {
            return m_List.Remove(item);
        }

        public void RemoveAt(int index)
        {
            m_List.RemoveAt(index);
        }

        public InstanceProto ToProtobuf()
        {
            InstanceProto instance = new InstanceProto();
            instance.Type = InstanceTypeProto.List;
            foreach (IInstance item in m_List)
            {
                instance.List.Add(item.ToProtobuf());
            }
            return instance;
        }

        public static object ParseFrom(RepeatedField<InstanceProto> listProto)
        {
            // Convert to List<object>
            List<object> list = new List<object>();
            foreach (InstanceProto proto in listProto)
            {
                list.Add(GymInstance.ParseFrom(proto));
            }
            return list;
        }

        public static implicit operator List<IInstance>(List list) => list.m_List;

        public override string ToString()
        {
            string output = "[";
            int i = 0;
            for (; i < m_List.Count - 1; i++)
            {
                output += m_List[i].ToString() + ", ";
            }
            if (i < m_List.Count) output += m_List[i].ToString();
            output += "]";
            return output;
        }
    }
}