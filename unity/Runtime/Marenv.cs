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
            if (field == null) field = "";
            List<string> fields = new List<string>{ field };
            AddObserver(fields, observer, scope);
        }

        public static void AddObserver(List<string> fields, IObserver observer, string scope = "")
        {
            FieldString scopeField = FieldString.ParseFrom(scope);
            List<FieldString> fieldStrings = FieldString.ParseFrom(fields);
            List<FieldString> fullFields = FieldString.Join(scopeField, fieldStrings);
            Instance._AddObserver(fullFields, observer);
        }

        public static void AddObserversFromObject(object o, string scope = "")
        {
            List<FieldString> scopes = new List<FieldString>{ FieldString.ParseFrom(scope) };
            Instance._AddObserversFromObject(o, scopes);
        }

        public static void AddObserversFromGameObject(object gameObject, string scope = "")
        {
            List<FieldString> scopes = new List<FieldString>{ FieldString.ParseFrom(scope) };
            Instance._AddObserversFromGameObject(gameObject, scopes);
        }

        public static void AddObserversFromComponent(object component, string scope = "")
        {
            List<FieldString> scopes = new List<FieldString>{ FieldString.ParseFrom(scope) };
            Instance._AddObserversFromComponent(component, scopes);
        }

        public static void SetObservation(string field, IData observation, string scope = "")
        {
            if (field == null) field = "";
            List<string> fields = new List<string>{ field };
            SetObservation(fields, observation, scope);
        }

        public static void SetObservation(List<string> fields, IData observation, string scope = "")
        {
            FieldString scopeField = FieldString.ParseFrom(scope);
            List<FieldString> fieldStrings = FieldString.ParseFrom(fields);
            List<FieldString> fullFields = FieldString.Join(scopeField, fieldStrings);
            Instance._SetObservation(fullFields, observation);
        }

        public static IData GetObservation(string agent)
        {
            return Instance._GetObservation(agent);
        }

        public static void Step()
        {
            Instance._Step();
        }

        Dictionary<FieldString, IObserver> m_Observers;
        Dictionary<FieldString, IData> m_Observations;
        int m_TimeStamp;
        Dictionary<IObserver, bool> m_Cached;

        private Marenv()
        {
            m_Observers = new Dictionary<FieldString, IObserver>();
            m_Observations = new Dictionary<FieldString, IData>();
            m_TimeStamp = 0;
            m_Cached = new Dictionary<IObserver, bool>();
        }

        void _Error(string reason)
        {
            throw new Exception("Marenv Error: " + reason);
        }

        internal void _Tick()
        {
            ++m_TimeStamp;
        }

        internal void _Step()
        {
            
        }

        void _AddObserver(List<FieldString> fieldStrings, IObserver observer)
        {
            foreach (FieldString fieldString in fieldStrings)
            {
                if (fieldString.IsRoot)
                {
                    m_Observers[fieldString] = observer;
                    // TODO: cache
                }
                else _Error("Field string should be absolute: " + fieldString.ToString());
            }
        }

        void _SetObservation(FieldString fieldString, IData observation)
        {
            if (fieldString.IsRoot)
            {
                m_Observations[fieldString] = observation;
            }
            else _Error("Field string should be absolute: " + fieldString.ToString());
        }

        void _SetObservation(List<FieldString> fieldStrings, IData observation)
        {
            foreach(FieldString fieldString in fieldStrings)
            {
                _SetObservation(fieldString, observation);
            }
        }

        IData _GetObservation(string agent)
        {
            _CollectObserverObservations(agent);
            IData mergedObservation = null;
            foreach (var fieldString_observation in m_Observations)
            {
                FieldString fieldString = fieldString_observation.Key;
                IData observation = fieldString_observation.Value;
                if (fieldString.HasAgent(agent))
                {
                    mergedObservation = _MergeObservation(mergedObservation, fieldString, observation);
                }
            }
            return mergedObservation;
        }

        void _CollectObserverObservations(string agent)
        {
            foreach (var fieldString_observer in m_Observers)
            {
                FieldString fieldString = fieldString_observer.Key;
                IObserver observer = fieldString_observer.Value;
                if (fieldString.HasAgent(agent))
                {
                    // TODO: cache
                    int cacheId = -1;
                    _SetObservation(fieldString, observer.GetObservation(cacheId));
                }
            }
        }

        IData _MergeObservation(IData original, FieldString fieldString, IData observation)
        {
            // TODO: 最難的整合
            return null;
        }

        void _AddObserversFromObject(object o, List<FieldString> scopes = null)
        {
            // Collect and add Observers from the "existed" object instance o
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (o != null)
            {
                // Get fields with Marenv Attribute
                FieldInfo[] fields = o.GetType().GetFields(bindingFlags);
                if (fields != null)
                {
                    foreach (FieldInfo field in fields)
                    {
                        AttributeBase attr = (AttributeBase)field.GetCustomAttribute(typeof(AttributeBase));
                        if (attr != null)
                        {
                            List<FieldString> fullFields = FieldString.Join(scopes, attr.GetFieldStrings(), field.Name);
                            _AddAttributeObserver(attr, fullFields, o, field);
                        }
                    }
                }

                // Get properties with Marenv Attribute
                PropertyInfo[] properties = o.GetType().GetProperties(bindingFlags);
                if (properties!= null)
                {
                    foreach (PropertyInfo prop in properties)
                    {
                        AttributeBase attr = (AttributeBase)prop.GetCustomAttribute(typeof(AttributeBase));
                        if (attr != null)
                        {
                            List<FieldString> fullFields = FieldString.Join(scopes, attr.GetFieldStrings(), prop.Name);
                            _AddAttributeObserver(attr, fullFields, o, prop);
                        }
                    }
                }
            }
        }

        void _AddObserversFromGameObject(object gameObject, List<FieldString> scopes = null)
        {
            GameObject o = gameObject as GameObject;
            if (o != null)
            {
                HashSet<FieldString> allFields = new HashSet<FieldString>();
                Component[] components = o.GetComponents(typeof(Component));
                foreach (Component component in components)
                {
                    ISensor sensor = component as ISensor;
                    if (sensor != null)
                    {
                        List<FieldString> fieldStrings = FieldString.ParseFrom(sensor.GetFields());
                        List<FieldString> fullFields = FieldString.Join(scopes, fieldStrings, o.name);
                        ScopeSensor scopeSensor = sensor as ScopeSensor;
                        if (scopeSensor == null) _AddObserver(fullFields, sensor);
                        // Collect fields in this level
                        allFields.UnionWith(fullFields);
                    }
                    _AddObserversFromObject(component, scopes);
                }
                List<FieldString> nextScopes = new List<FieldString>(allFields);
                for (int i = 0; i < o.transform.childCount; i++)
                {
                    GameObject child = o.transform.GetChild(i).gameObject;
                    _AddObserversFromGameObject(child, nextScopes);
                }
            }
        }

        void _AddObserversFromComponent(object component, List<FieldString> scopes = null)
        {
            Component o = component as Component;
            if (o != null)
            {
                _AddObserversFromGameObject(o.gameObject, scopes);
            }
        }

        void _AddAttributeObserver(AttributeBase attr, List<FieldString> fields, object o, MemberInfo memberInfo)
        {
            AttributeObserver observer = new AttributeObserver(attr, o, memberInfo);
            _AddObserver(fields, observer);
        }

        public static void TestCollectObservers(object o)
        {
            // TODO: Delete this method after testing
            AddObserversFromComponent(o, "agent@");
            foreach (var kv in Instance.m_Observers)
            {
                Debug.Log(kv.Key);
            }
            Instance._CollectObserverObservations("agent");
            foreach (var kv in Instance.m_Observations)
            {
                Debug.Log(kv.Key);
            }
        }
    }
}