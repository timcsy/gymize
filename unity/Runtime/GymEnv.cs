using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NumSharp;
using UnityEngine;
using Google.Protobuf;
using Gymize.Protobuf;

namespace Gymize
{
    public delegate void ResetCallBack();
    public delegate void InfoCallBack(object info);

    public class GymEnv : GymChannel
    {
        // Lazy initializer pattern, see https://csharpindepth.com/articles/singleton#lazy
        private static Lazy<GymEnv> s_Lazy = new Lazy<GymEnv>(() => new GymEnv());

        public static GymEnv Instance { get { return s_Lazy.Value; } }

        public static bool IsInitialized
        {
            get { return s_Lazy.IsValueCreated; }
        }

        public static DelegateDictionary<string, ResetCallBack> OnReset
        {
            get { return Instance.m_OnReset; }
            set { Instance.m_OnReset = value; }
        }
        public static DelegateDictionary<string, ActionCallBack> OnAction
        {
            get { return Instance.m_OnAction; }
            set { Instance.m_OnAction = value; }
        }
        public static Dictionary<string, object> Actions
        {
            get { return Instance.m_Actions; }
        }
        public static Dictionary<string, double> Rewards
        {
            get { return Instance.m_Rewards; }
        }
        public static DelegateDictionary<string, InfoCallBack> OnInfo
        {
            get { return Instance.m_OnInfo; }
            set { Instance.m_OnInfo = value; }
        }

        public static void Start(string name)
        {
            Instance.StartChannel(name);
        }

        public static List<IObserver> AddAgent(IAgent agent)
        {
            return Instance._AddAgent(agent);
        }

        public static void RemoveAgent(IAgent agent, List<IObserver> observers)
        {
            Instance._RemoveAgent(agent, observers);
        }

        public static void AddActuator(IActuator actuator)
        {
            Instance._AddActuator(actuator);
        }

        public static void RemoveActuator(string name)
        {
            Instance._RemoveActuator(name);
        }

        public static void RemoveActuator(IActuator actuator)
        {
            Instance._RemoveActuator(actuator);
        }

        public static object GetAction(string name)
        {
            return Instance._GetAction(name);
        }

        public static void AddObserver(IObserver observer, string scope = "")
        {
            Locator fullLoc = Locator.Join(scope, observer.GetLocator());
            Instance._AddObserver(fullLoc, observer);
        }

        public static void AddObserver(string locator, IObserver observer, string scope = "")
        {
            Locator fullLoc = Locator.Join(scope, locator);
            Instance._AddObserver(fullLoc, observer);
        }

        public static List<IObserver> AddObserversFromObject(object o, string scope = "")
        {
            Locator scopeLoc = Locator.ParseFrom(scope);
            return Instance._AddObserversFromObject(o, scopeLoc);
        }

        public static List<IObserver> AddObserversFromGameObject(object gameObject, string scope = "")
        {
            Locator scopeLoc = Locator.ParseFrom(scope);
            return Instance._AddObserversFromGameObject(gameObject, scopeLoc);
        }

        public static List<IObserver> AddObserversFromComponent(object component, string scope = "")
        {
            Locator scopeLoc = Locator.ParseFrom(scope);
            return Instance._AddObserversFromComponent(component, scopeLoc);
        }

        public static void RemoveObserver(string locator)
        {
            Instance._RemoveObserver(Locator.ParseFrom(locator));
        }

        public static void RemoveObserver(Locator locator)
        {
            Instance._RemoveObserver(locator);
        }

        public static void RemoveObserver(IObserver observer)
        {
            Instance._RemoveObserver(observer);
        }

        public static void SetObservation(string locator, object observation, string scope = "")
        {
            Locator fullLoc = Locator.Join(scope, locator);
            Instance._SetObservation(fullLoc, observation);
        }

        public static void AddReward(string agent, double reward)
        {
            Instance._AddReward(agent, reward);
        }

        public static void SetReward(string agent, double reward)
        {
            Instance._SetReward(agent, reward);
        }

        public static void Terminate(string agent = "")
        {
            Instance._Terminate(agent);
        }

        public static void Truncate(string agent = "")
        {
            Instance._Truncate(agent);
        }

        public static void SendInfo(object info)
        {
            Instance._SendInfo("", info);
        }

        public static void SendInfo(string agent, object info)
        {
            Instance._SendInfo(agent, info);
        }

        public static void Step()
        {
            Instance._Step();
        }

        public static void Close()
        {
            Instance.CloseChannel();
        }

        public static void Error(string reason)
        {
            throw new Exception("Gymize Error: " + reason);
        }

        DelegateDictionary<string, ResetCallBack> m_OnReset;
        DelegateDictionary<string, ActionCallBack> m_OnAction;
        Dictionary<string, object> m_Actions;
        Dictionary<Locator, List<IObserver>> m_Observers;
        Dictionary<Locator, List<IInstance>> m_Observations;
        Dictionary<string, double> m_Rewards;
        List<string> m_TerminatedAgents;
        List<string> m_TruncationAgents;
        DelegateDictionary<string, InfoCallBack> m_OnInfo;
        Dictionary<string, List<object>> m_Infos;

        private GymEnv() : base()
        {
            m_OnReset = new DelegateDictionary<string, ResetCallBack>()
            {
                { "", null}
            };
            m_OnAction = new DelegateDictionary<string, ActionCallBack>();
            m_Actions = new Dictionary<string, object>();
            m_Observers = new Dictionary<Locator, List<IObserver>>();
            m_Observations = new Dictionary<Locator, List<IInstance>>();
            m_Rewards = new Dictionary<string, double>();
            m_TerminatedAgents = new List<string>();
            m_TruncationAgents = new List<string>();
            m_OnInfo = new DelegateDictionary<string, InfoCallBack>()
            {
                { "", null }
            };
            m_Infos = new Dictionary<string, List<object>>()
            {
                { "", new List<object>() }
            };
        }

        List<IObserver> _AddAgent(IAgent agent)
        {
            m_OnReset[agent.GetName()] += agent.OnReset;
            m_OnInfo[agent.GetName()] += agent.OnInfo;
            _AddActuator(agent);
            Locator scope = Locator.ParseFrom(agent.GetName() + "@");
            return _AddObserversFromComponent(agent, scope);
        }

        void _RemoveAgent(IAgent agent, List<IObserver> observers)
        {
            m_OnReset[agent.GetName()] -= agent.OnReset;
            m_OnInfo[agent.GetName()] -= agent.OnInfo;
            _RemoveActuator(agent);
            foreach (IObserver observer in observers)
            {
                _RemoveObserver(observer);
            }
        }

        void _Reset(GymizeProto gymizeProto)
        {
            // TODO: Also reset actions, observations, rewards, termiantions, truncations, infos
            foreach (string agent in gymizeProto.ResetAgents)
            {
                m_Actions[agent] = null;
                m_Rewards[agent] = 0;
                m_TerminatedAgents.Remove(agent);
                m_TerminatedAgents.Remove(agent);
                m_Infos[agent] = new List<object>();
                m_OnReset[agent]?.Invoke();
            }
        }

        void _AddActuator(IActuator actuator)
        {
            m_OnAction[actuator.GetName()] += actuator.OnAction;
        }

        void _RemoveActuator(string name)
        {
            m_OnAction[name] = null;
        }

        void _RemoveActuator(IActuator actuator)
        {
            m_OnAction[actuator.GetName()] -= actuator.OnAction;
        }

        object _GetAction(string name)
        {
            if (m_Actions.ContainsKey(name))
            {
                return m_Actions[name];
            }
            else Debug.LogWarning(name + " is not a valid action name");
            return null;
        }

        void _SetActions(GymizeProto gymizeProto)
        {
            foreach (ActionProto actionProto in gymizeProto.Actions)
            {
                m_Actions[actionProto.Agent] = GymInstance.ParseFrom(actionProto.Action);
                m_OnAction[actionProto.Agent]?.Invoke(_GetAction(actionProto.Agent));
            }
        }

        void _AddObserver(Locator locator, IObserver observer)
        {
            foreach(Mapping mapping in locator.Mappings)
            {
                if (!mapping.IsRoot)
                {
                    Error("Locator should be absolute: " + mapping.ToString() + ", in " + locator.ToString());
                    return;
                }
            }
            if (!m_Observers.ContainsKey(locator)) m_Observers[locator] = new List<IObserver>();
            if (!locator.HasSequence()) m_Observers[locator].Clear();
            m_Observers[locator].Add(observer);
        }

        List<IObserver> _AddObserversFromObject(object o, Locator scope = null)
        {
            // Collect and add Observers from the "existed" object instance o

            List<IObserver> observers = new List<IObserver>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (o != null)
            {
                // Get fields with Gymize Attribute
                FieldInfo[] fields = o.GetType().GetFields(bindingFlags);
                if (fields != null)
                {
                    foreach (FieldInfo field in fields)
                    {
                        AttributeBase attr = (AttributeBase)field.GetCustomAttribute(typeof(AttributeBase));
                        if (attr != null)
                        {
                            Locator loc = Locator.ParseFrom(attr.GetLocator());
                            Locator fullLoc = Locator.Join(scope, loc, field.Name);
                            observers.Add(_AddAttributeObserver(attr, fullLoc, o, field));
                        }
                    }
                }

                // Get properties with Gymize Attribute
                PropertyInfo[] properties = o.GetType().GetProperties(bindingFlags);
                if (properties!= null)
                {
                    foreach (PropertyInfo prop in properties)
                    {
                        AttributeBase attr = (AttributeBase)prop.GetCustomAttribute(typeof(AttributeBase));
                        if (attr != null)
                        {
                            Locator loc = Locator.ParseFrom(attr.GetLocator());
                            Locator fullLoc = Locator.Join(scope, loc, prop.Name);
                            observers.Add(_AddAttributeObserver(attr, fullLoc, o, prop));
                        }
                    }
                }
            }
            return observers;
        }

        List<IObserver> _AddObserversFromGameObject(object gameObject, Locator scope = null)
        {
            List<IObserver> observers = new List<IObserver>();

            GameObject o = gameObject as GameObject;
            if (o != null)
            {
                HashSet<Mapping> allMappings = new HashSet<Mapping>();
                Component[] components = o.GetComponents(typeof(Component));
                foreach (Component component in components)
                {
                    Behaviour behaviour = component as Behaviour;
                    if (behaviour != null && !behaviour.enabled) continue;

                    IObserver observer = component as IObserver;
                    if (observer != null)
                    {
                        Locator locator = Locator.ParseFrom(observer.GetLocator());
                        Locator fullLoc = Locator.Join(scope, locator, o.name);
                        ScopeSensor scopeSensor = observer as ScopeSensor;
                        if (scopeSensor == null)
                        {
                            _AddObserver(fullLoc, observer);
                            observers.Add(observer);
                        }
                        // Collect locations in this level
                        allMappings.UnionWith(fullLoc.Mappings);
                    }
                    observers.AddRange(_AddObserversFromObject(component, scope));
                }
                Locator nextScope = new Locator();
                nextScope.Mappings.AddRange(allMappings);
                for (int i = 0; i < o.transform.childCount; i++)
                {
                    GameObject child = o.transform.GetChild(i).gameObject;
                    observers.AddRange(_AddObserversFromGameObject(child, nextScope));
                }
            }
            return observers;
        }

        List<IObserver> _AddObserversFromComponent(object component, Locator scope = null)
        {
            Component o = component as Component;
            if (o != null)
            {
                return _AddObserversFromGameObject(o.gameObject, scope);
            }
            else return new List<IObserver>();
        }

        IObserver _AddAttributeObserver(AttributeBase attr, Locator locator, object o, MemberInfo memberInfo)
        {
            AttributeObserver observer = new AttributeObserver(attr, o, memberInfo);
            _AddObserver(locator, observer);
            return observer;
        }

        void _RemoveObserver(Locator locator)
        {
            m_Observers.Remove(locator);
        }

        void _RemoveObserver(IObserver observer)
        {
            foreach (KeyValuePair<Locator, List<IObserver>> kvp in m_Observers)
            {
                List<IObserver> observers = kvp.Value;
                observers.RemoveAll(o => o == observer);
            }
        }

        void _SetObservation(Locator locator, object observation)
        {
            foreach(Mapping mapping in locator.Mappings)
            {
                if (!mapping.IsRoot)
                {
                    Error("Locator should be absolute: " + mapping.ToString() + ", in " + locator.ToString());
                    return;
                }
            }
            if (!m_Observations.ContainsKey(locator)) m_Observations[locator] = new List<IInstance>();
            if (!locator.HasSequence()) m_Observations[locator].Clear();
            m_Observations[locator].Add(GymInstance.ToGym(observation));
        }

        List<ObservationProto> _GetObservations()
        {
            // TODO: just send the requested agents
            List<ObservationProto> observationProtos = new List<ObservationProto>();
            foreach (var locator_observations in m_Observations)
            {
                Locator locator = locator_observations.Key;
                List<IInstance> observations = locator_observations.Value;
                foreach (IInstance observation in observations)
                {
                    ObservationProto observationProto = new ObservationProto();
                    observationProto.Locator = locator.ToProtobuf();
                    if (observation == null) Debug.LogWarning(locator + " is null observation");
                    else observationProto.Observation = observation.ToProtobuf();
                    observationProtos.Add(observationProto);
                }
            }
            // TODO: whether to clear all agents?
            m_Observations.Clear();
            foreach (var locator_observers in m_Observers)
            {
                Locator locator = locator_observers.Key;
                List<IObserver> observers = locator_observers.Value;
                foreach (IObserver observer in observers)
                {
                    ObservationProto observationProto = new ObservationProto();
                    observationProto.Locator = locator.ToProtobuf();
                    IInstance observation = observer.GetObservation();
                    if (observation == null) Debug.LogWarning(locator.ToString() + " is null observation");
                    else observationProto.Observation = observation.ToProtobuf();
                    observationProtos.Add(observationProto);
                }
            }
            return observationProtos;
        }

        void _AddReward(string agent, double reward)
        {
            if (!m_Rewards.ContainsKey(agent)) _ClearReward(agent);
            m_Rewards[agent] += reward;
        }

        void _SetReward(string agent, double reward)
        {
            m_Rewards[agent] = reward;
        }

        void _ClearReward(string agent)
        {
            // TODO: when should be cleared?
            m_Rewards[agent] = 0;
        }

        List<RewardProto> _GetRewards()
        {
            // TODO: just send the requested agents
            List<RewardProto> rewardProtos = new List<RewardProto>();
            foreach (string agent in m_Rewards.Keys.ToList<string>())
            {
                RewardProto rewardProto = new RewardProto();
                rewardProto.Agent = agent;
                rewardProto.Reward = m_Rewards[agent];
                rewardProtos.Add(rewardProto);
                _ClearReward(agent);
            }
            return rewardProtos;
        }

        void _Terminate(string agent)
        {
            m_TerminatedAgents.Add(agent);
        }

        List<string> _GetTerminations()
        {
            // TODO: when terminations should be included? or when should be reset? 已經停掉的也不會送東西過來了
            List<string> terminatedAgents = m_TerminatedAgents;
            m_TerminatedAgents = new List<string>();
            return terminatedAgents;
        }

        void _Truncate(string agent)
        {
            m_TruncationAgents.Add(agent);
        }

        List<string> _GetTruncations()
        {
            // TODO: when truncations should be included? or when should be reset? 已經停掉的也不會送東西過來了
            List<string> truncatedAgents = m_TruncationAgents;
            m_TruncationAgents = new List<string>();
            return truncatedAgents;
        }

        void _SendInfo(string agent, object info)
        {
            if (!m_Infos.ContainsKey(agent)) m_Infos[agent] = new List<object>();
            m_Infos[agent].Add(info);
        }

        void _RecvInfos(GymizeProto gymizeProto)
        {
            foreach (InfoProto infoProto in gymizeProto.Infos)
            {
                foreach (InstanceProto instanceProto in infoProto.Infos)
                {
                    object obj = GymInstance.ParseFrom(instanceProto);
                    m_OnInfo[infoProto.Agent]?.Invoke(obj);
                }
            }
        }

        List<InfoProto> _GetInfos()
        {
            // TODO: when info should be cleared? and when env "" info be cleared?
            // TODO: just send the requested agents
            List<InfoProto> infoProtos = new List<InfoProto>();
            foreach (string agent in m_Infos.Keys.ToList<string>())
            {
                InfoProto infoProto = new InfoProto();
                infoProto.Agent = agent;
                List<object> infos = m_Infos[agent];
                foreach (object obj in infos)
                {
                    infoProto.Infos.Add(GymInstance.ToGym(obj).ToProtobuf());
                }
                infoProtos.Add(infoProto);
                m_Infos[agent] = new List<object>();
            }
            return infoProtos;
        }

        internal void _CheckChannel()
        {
            if (HasMessage("_gym_"))
            {
                Content content = TakeMessage("_gym_");
                if (content.IsBinary)
                {
                    GymizeProto gymizeProto = GymizeProto.Parser.ParseFrom(content.Raw);
                    _RecvGymizeMessage(gymizeProto);
                }
                _SendGymizeMessage();
            }
            else QuitWhenChannelClosed();
        }

        internal void _Step()
        {
            
        }

        void _RecvGymizeMessage(GymizeProto gymizeProto)
        {
            _Reset(gymizeProto);
            _SetActions(gymizeProto);
            _RecvInfos(gymizeProto);
        }

        void _SendGymizeMessage()
        {
            GymizeProto gymizeProto = new GymizeProto();
            gymizeProto.Observations.AddRange(_GetObservations());
            gymizeProto.Rewards.AddRange(_GetRewards());
            gymizeProto.TerminatedAgents.AddRange(_GetTerminations());
            gymizeProto.TruncatedAgents.AddRange(_GetTruncations());
            gymizeProto.Infos.AddRange(_GetInfos());
            byte[] gymizeMessage = gymizeProto.ToByteArray();
            TellSync("_gym_", gymizeMessage);
        }
    }
}