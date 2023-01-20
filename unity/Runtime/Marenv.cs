using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PAIA.Marenv
{
    public class Marenv
    {
        // Lazy initializer pattern, see https://csharpindepth.com/articles/singleton#lazy
        private static Lazy<Marenv> s_Lazy = new Lazy<Marenv>(() => new Marenv());

        public static Marenv Instance { get { return s_Lazy.Value; } }

        public static bool IsInitialized
        {
            get { return s_Lazy.IsValueCreated; }
        }

        public static void AddObserver(string field, IObserver observer, string scope = "")
        {
            Instance._AddObserver(field, observer, scope);
        }

        public static void AddObserver(List<string> fields, IObserver observer, string scope = "")
        {
            Instance._AddObserver(fields, observer, scope);
        }

        public static IData GetObservation(string agent)
        {
            return Instance._GetObservation(agent);
        }

        public static void SetObservation(string field, IData observation, string scope = "")
        {
            List<string> fields = new List<string>();
            fields.Add(field);
            SetObservation(fields, observation, scope);
        }

        public static void SetObservation(List<string> fields, IData observation, string scope = "")
        {
            List<FieldString> fieldStrings = new List<FieldString>();
            foreach (string field in fields) fieldStrings.Add(FieldString.ParseFrom(field));
            Instance._SetObservation(fieldStrings, observation, scope);
        }

        public static void Step()
        {
            Instance._Step();
        }

        Dictionary<string, IAgent> m_Agents;
        List<IObserver> m_Observers;
        Dictionary<IObserver, bool> m_Cached;
        int m_TimeStamp;

        private Marenv()
        {
            m_Agents = new Dictionary<string, IAgent>();
            m_Observers = new List<IObserver>();
            m_Cached = new Dictionary<IObserver, bool>();
            m_TimeStamp = 0;
        }

        internal void _AddObserver(string field, IObserver observer, string scope = "")
        {
            if (field == null) field = "";
            List<string> fields = new List<string>();
            fields.Add(field);
            _AddObserver(fields, observer, scope);
        }

        internal void _AddObserver(List<string> fields, IObserver observer, string scope = "")
        {
            List<FieldString> fieldStrings = new List<FieldString>();
            if (fields != null)
            {
                foreach(string field in fields)
                {
                    fieldStrings.Add(FieldString.ParseFrom(field));
                }
            }
            // TODO
        }

        internal IData _GetObservation(string agent)
        {
            return null;
        }

        internal void _SetObservation(List<FieldString> fields, IData observation, string scope = "")
        {
            
        }

        internal void Tick()
        {
            ++m_TimeStamp;
        }

        internal void _Step()
        {
            
        }

        public static void TestCollectObservers(object o)
        {
            // TODO: Delete this method after testing
            Instance.CollectObservers(o);
        }

        void CollectObservers(object o)
        {
            GetChildrenGameObjects((o as MonoBehaviour).gameObject);
            Debug.Log("GetType: " + o.GetType());
            // Which scope to search
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            // Get fields with DataAttribute
            FieldInfo[] fields = o.GetType().GetFields(bindingFlags);
            foreach (FieldInfo field in fields)
            {
                AttributeBase attr = (AttributeBase)field.GetCustomAttribute(typeof(AttributeBase));
                if (attr != null)
                {
                    string declaringTypeName = field.DeclaringType.Name;
                    string memberName = field.Name;
                    Type memberType = field.FieldType;
                    Debug.Log("Field:");
                    Debug.Log("DeclaringTypeName: " + declaringTypeName);
                    Debug.Log("MemberName: " + memberName); // Default Field name for ObservationInfo.Fields
                    Debug.Log("MemberType: " + memberType.ToString());
                    Debug.Log("AttributeField: " + attr.Fields); // Add to ObservationInfo.Fields
                }
            }
            // Get properties with DataAttribute
            PropertyInfo[] properties = o.GetType().GetProperties(bindingFlags);
            foreach (PropertyInfo prop in properties)
            {
                AttributeBase attr = (AttributeBase)prop.GetCustomAttribute(typeof(AttributeBase));
                if (attr != null)
                {
                    string declaringTypeName = prop.DeclaringType.Name;
                    string memberName = prop.Name;
                    Type memberType = prop.PropertyType;
                    Debug.Log("Property:");
                    Debug.Log("DeclaringTypeName: " + declaringTypeName);
                    Debug.Log("MemberName: " + memberName);
                    Debug.Log("MemberType: " + memberType.ToString());
                    Debug.Log("AttributeField: " + attr.Fields);
                }
            }
            // TODO: Get Data from child sensor componentss
            
        }

        void GetChildrenGameObjects(object o)
        {
            GameObject parent = o as GameObject;
            if (parent != null)
            {
                for (int i = 0; i < parent.transform.childCount; i++)
                {
                    GameObject child = parent.transform.GetChild(i).gameObject;
                    Debug.Log("Child name: " + child.name);
                    GetChildrenGameObjects(child);
                }
            }
        }
    }
}