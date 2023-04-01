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

        public static void AddObserver(string location, IObserver observer, string scope = "")
        {
            if (location == null) location = "";
            List<string> locations = new List<string>{ location };
            AddObserver(locations, observer, scope);
        }

        public static void AddObserver(List<string> locations, IObserver observer, string scope = "")
        {
            Location scopeLoc = Location.ParseFrom(scope);
            List<Location> locs = Location.ParseFrom(locations);
            List<Location> fullLocs = Location.Join(scopeLoc, locs);
            Instance._AddObserver(fullLocs, observer);
        }

        public static void AddAgent(IAgent agent)
        {
            // TODO: deal with the agent name and scope, and AddObserversFromObject or AddObserversFromComponent
        }

        public static void AddObserversFromObject(object o, string scope = "")
        {
            List<Location> scopes = new List<Location>{ Location.ParseFrom(scope) };
            Instance._AddObserversFromObject(o, scopes);
        }

        public static void AddObserversFromGameObject(object gameObject, string scope = "")
        {
            List<Location> scopes = new List<Location>{ Location.ParseFrom(scope) };
            Instance._AddObserversFromGameObject(gameObject, scopes);
        }

        public static void AddObserversFromComponent(object component, string scope = "")
        {
            List<Location> scopes = new List<Location>{ Location.ParseFrom(scope) };
            Instance._AddObserversFromComponent(component, scopes);
        }

        public static void SetObservation(string location, IData observation, string scope = "")
        {
            if (location == null) location = "";
            List<string> locations = new List<string>{ location };
            SetObservation(locations, observation, scope);
        }

        public static void SetObservation(List<string> locations, IData observation, string scope = "")
        {
            Location scopeLoc = Location.ParseFrom(scope);
            List<Location> locs = Location.ParseFrom(locations);
            List<Location> fullLocs = Location.Join(scopeLoc, locs);
            Instance._SetObservation(fullLocs, observation);
        }

        public static IData GetObservations(string agent)
        {
            return Instance._GetObservations(agent);
        }

        public static void Step()
        {
            Instance._Step();
        }

        public static void Error(string reason)
        {
            throw new Exception("Marenv Error: " + reason);
        }

        Dictionary<Location, IObserver> m_Observers;
        Dictionary<Location, IData> m_Observations;
        int m_TimeStamp;
        Dictionary<IObserver, bool> m_Cached;

        private Marenv()
        {
            m_Observers = new Dictionary<Location, IObserver>();
            m_Observations = new Dictionary<Location, IData>();
            m_TimeStamp = 0;
            m_Cached = new Dictionary<IObserver, bool>();
        }

        internal void _Tick()
        {
            ++m_TimeStamp;
        }

        internal void _Step()
        {
            
        }

        void _AddObserver(List<Location> locations, IObserver observer)
        {
            foreach (Location location in locations)
            {
                if (location.IsRoot)
                {
                    m_Observers[location] = observer;
                    // TODO: cache
                }
                else Error("Location string should be absolute: " + location.ToString());
            }
        }

        void _SetObservation(Location location, IData observation)
        {
            if (location.IsRoot)
            {
                m_Observations[location] = observation;
            }
            else Error("Location string should be absolute: " + location.ToString());
        }

        void _SetObservation(List<Location> locations, IData observation)
        {
            foreach(Location location in locations)
            {
                _SetObservation(location, observation);
            }
        }

        IData _GetObservations(string agent)
        {
            _CollectObserverObservations(agent);
            IData mergedObservation = null;
            foreach (var location_observation in m_Observations)
            {
                Location location = location_observation.Key;
                IData observation = location_observation.Value;
                if (location.HasAgent(agent))
                {
                    mergedObservation = _MergeObservations(mergedObservation, location, observation);
                }
            }
            return mergedObservation;
        }

        void _CollectObserverObservations(string agent)
        {
            foreach (var location_observer in m_Observers)
            {
                Location location = location_observer.Key;
                IObserver observer = location_observer.Value;
                if (location.HasAgent(agent))
                {
                    // TODO: cache
                    int cacheId = -1;
                    _SetObservation(location, observer.GetObservation(cacheId));
                }
            }
        }

        IData _MergeObservations(IData original, Location location, IData observation)
        {
            // location is already absolute in this system
            if (location.Paths.Count == 0)
            {
                // Only mapping (or even no mapping)
                if (observation == null) return original;
                return observation.Merge(original, location.Mapping);
            }
            else
            {
                // Having paths
                if (original == null)
                {
                    // Create a empty original data
                    switch (location.Paths[0].Type)
                    {
                        case LocationType.DICT: original = new Dict(); break;
                        case LocationType.TUPLE: original = new Tuple(); break;
                        case LocationType.SEQUENCE: original = new Sequence(); break;
                        default: Error("Wrong location path type"); break;
                    }
                }

                // Get the path before last path
                IData current = original;
                IComposite composite;
                for (int i = 0; i < location.Paths.Count - 1; i++)
                {
                    composite = current as IComposite;
                    if (composite == null)
                    {
                        Error("Wrong data structure mapping when processing location string: " + location.ToString());
                        return null;
                    }
                    current = composite.Select(location.Paths[i]);
                    if (current == null)
                    {
                        // Create path if not exists
                        switch (location.Paths[i+1].Type)
                        {
                            case LocationType.DICT: current = new Dict(); break;
                            case LocationType.TUPLE: current = new Tuple(); break;
                            case LocationType.SEQUENCE: current = new Sequence(); break;
                            default: Error("Wrong location path type"); break;
                        }
                        composite.Set(location.Paths[i], current);
                        current = composite.Select(location.Paths[i]);
                    }
                }
                // last path
                Path path = location.Paths[location.Paths.Count - 1];
                composite = current as IComposite;
                if (composite == null)
                {
                    Error("Wrong data structure mapping when processing location string: " + location.ToString());
                    return null;
                }
                current = composite.Select(path);
                if (observation != null) current = observation.Merge(current, location.Mapping);
                composite.Set(path, current);
                return original;
            }
        }

        void _AddObserversFromObject(object o, List<Location> scopes = null)
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
                            List<Location> locs = Location.ParseFrom(attr.GetLocations());
                            List<Location> fullLocs = Location.Join(scopes, locs, field.Name);
                            _AddAttributeObserver(attr, fullLocs, o, field);
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
                            List<Location> locs = Location.ParseFrom(attr.GetLocations());
                            List<Location> fullLocs = Location.Join(scopes, locs, prop.Name);
                            _AddAttributeObserver(attr, fullLocs, o, prop);
                        }
                    }
                }
            }
        }

        void _AddObserversFromGameObject(object gameObject, List<Location> scopes = null)
        {
            GameObject o = gameObject as GameObject;
            if (o != null)
            {
                HashSet<Location> allLocs = new HashSet<Location>();
                Component[] components = o.GetComponents(typeof(Component));
                foreach (Component component in components)
                {
                    ISensor sensor = component as ISensor;
                    if (sensor != null)
                    {
                        List<Location> locations = Location.ParseFrom(sensor.GetLocations());
                        List<Location> fullLocs = Location.Join(scopes, locations, o.name);
                        ScopeSensor scopeSensor = sensor as ScopeSensor;
                        if (scopeSensor == null) _AddObserver(fullLocs, sensor);
                        // Collect locations in this level
                        allLocs.UnionWith(fullLocs);
                    }
                    _AddObserversFromObject(component, scopes);
                }
                List<Location> nextScopes = new List<Location>(allLocs);
                for (int i = 0; i < o.transform.childCount; i++)
                {
                    GameObject child = o.transform.GetChild(i).gameObject;
                    _AddObserversFromGameObject(child, nextScopes);
                }
            }
        }

        void _AddObserversFromComponent(object component, List<Location> scopes = null)
        {
            Component o = component as Component;
            if (o != null)
            {
                _AddObserversFromGameObject(o.gameObject, scopes);
            }
        }

        void _AddAttributeObserver(AttributeBase attr, List<Location> locations, object o, MemberInfo memberInfo)
        {
            AttributeObserver observer = new AttributeObserver(attr, o, memberInfo);
            _AddObserver(locations, observer);
        }

        public static void TestCollectObservers(object o)
        {
            // TODO: Delete this method after testing
            // AddObserversFromComponent(o, "agent@");
            // foreach (var kv in Instance.m_Observers)
            // {
            //     Debug.Log(kv.Key);
            // }
            // Instance._CollectObserverObservations("agent");
            // foreach (var kv in Instance.m_Observations)
            // {
            //     Debug.Log(kv.Key);
            // }
            Instance._CollectObserverObservations("kart1");
            foreach (var kv in Instance.m_Observations)
            {
                Debug.Log(kv.Key);
            }
        }
    }
}