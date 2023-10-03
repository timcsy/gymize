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

        public static GymRender Render
        {
            get { return Instance.m_Render; }
        }

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

        public static void AddSensor(SensorComponent sensor)
        {
            Instance._AddSensor(sensor);
        }

        public static List<IObserver> AddObserversFromAgent(IAgent agent)
        {
            return Instance._AddObserversFromAgent(agent);
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

        public static void SetPeriod(string agent, int period)
        {
            // agent includes env ""
            Instance._SetPeriod(agent, period);
        }

        public static void Tick()
        {
            Instance._Tick();
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

        Dictionary<string, bool> m_Requested; // Whether another side has requested
        Dictionary<string, bool> m_UpdateAgents; // TODO v: store agents that meets update period
        Dictionary<string, int> m_Periods; // TODO v: How long (ticks) does agent reacts
        Dictionary<string, int> m_Ticks; // TODO v: How many ticks does an agent has passed since reacts

        GymRender m_Render;
        List<string> m_RequestViews;

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
            m_OnInfo = new DelegateDictionary<string, InfoCallBack>
            {
                { "", null }
            };
            m_Infos = new Dictionary<string, List<object>>
            {
                { "", new List<object>() }
            };

            m_Requested = new Dictionary<string, bool>
            {
                { "", false }
            };
            m_UpdateAgents = new Dictionary<string, bool>
            {
                { "", false }
            };
            m_Periods = new Dictionary<string, int>()
            {
                { "", 1 } // You have set it again in GymManager
            };
            m_Ticks = new Dictionary<string, int>
            {
                { "", 0 }
            };

            m_Render = new GymRender();
            m_RequestViews = new List<string>();
        }

        List<IObserver> _AddAgent(IAgent agent)
        {
            _SetPeriod(agent.GetName(), agent.GetStepPeriod());
            m_Ticks[agent.GetName()] = 0;
            m_UpdateAgents[agent.GetName()] = false;

            m_OnReset[agent.GetName()] += agent.OnReset;
            m_OnInfo[agent.GetName()] += agent.OnInfo;
            _AddActuator(agent);
            return _AddObserversFromAgent(agent);
        }

        void _RemoveAgent(IAgent agent, List<IObserver> observers)
        {
            _RemovePeriod(agent.GetName());
            m_Ticks.Remove(agent.GetName());
            m_UpdateAgents.Remove(agent.GetName());

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
            // TODO v: Also reset actions, "observations", rewards, termiantions, truncations, infos
            foreach (string agent in gymizeProto.ResetAgents)
            {
                m_Ticks[agent] = 0;
                m_UpdateAgents[agent] = false;

                m_Actions[agent] = null;
                _RemoveObservation(agent);
                m_Rewards[agent] = 0;
                m_TerminatedAgents.RemoveAll(x => x == agent);
                m_TerminatedAgents.RemoveAll(x => x == agent);
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

        void _AddSensor(SensorComponent sensor)
        {
            // Add Observer from SensorComponent
            Locator scope = new Locator();

            GameObject gameObject = sensor.gameObject;
            string prefix = ""; // path between the GameObject of Agent (excluded) and Sensor (included)
            // find parent, grand parent, ...
            // Collect agent names w.r.t. its scope
            while (gameObject != null)
            {
                IAgent[] agents = gameObject.GetComponents<IAgent>();
                string agentNames = "";
                foreach (IAgent agent in agents) agentNames += agent.GetName() + "@";
                if (agents.Count<IAgent>() > 0)
                {
                    scope.Mappings.AddRange(Locator.ParseFrom(agentNames + prefix).Mappings);
                }
                prefix = "[\'" + gameObject.name + "\']" + prefix;
                gameObject = gameObject.transform.parent?.gameObject;
            }

            Locator locator = Locator.ParseFrom(sensor.GetLocator());
            Locator fullLoc = Locator.Join(scope, locator);
            _AddObserver(fullLoc, sensor);
        }

        List<IObserver> _AddObserversFromAgent(IAgent agent)
        {
            // Collect and add Attribute Observers from an agent
            List<IObserver> observers = new List<IObserver>();
            
            Locator scope = Locator.ParseFrom(agent.GetName() + "@");

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (agent != null)
            {
                // Get fields with Gymize Attribute
                FieldInfo[] fields = agent.GetType().GetFields(bindingFlags);
                if (fields != null)
                {
                    foreach (FieldInfo field in fields)
                    {
                        AttributeBase attr = (AttributeBase)field.GetCustomAttribute(typeof(AttributeBase));
                        if (attr != null)
                        {
                            Locator loc = Locator.ParseFrom(attr.GetLocator());
                            Locator fullLoc = Locator.Join(scope, loc, field.Name);
                            observers.Add(_AddAttributeObserver(attr, fullLoc, agent, field));
                        }
                    }
                }

                // Get properties with Gymize Attribute
                PropertyInfo[] properties = agent.GetType().GetProperties(bindingFlags);
                if (properties!= null)
                {
                    foreach (PropertyInfo prop in properties)
                    {
                        AttributeBase attr = (AttributeBase)prop.GetCustomAttribute(typeof(AttributeBase));
                        if (attr != null)
                        {
                            Locator loc = Locator.ParseFrom(attr.GetLocator());
                            Locator fullLoc = Locator.Join(scope, loc, prop.Name);
                            observers.Add(_AddAttributeObserver(attr, fullLoc, agent, prop));
                        }
                    }
                }
            }
            return observers;
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

        List<ObservationProto> _GetObservations(List<string> responseAgents)
        {
            // TODO v: just send the requested agents
            // TODO v: whether to clear all agents? After sendiong
            List<ObservationProto> observationProtos = new List<ObservationProto>();
            foreach (string agent in responseAgents)
            {
                foreach (var locator_observers in m_Observers)
                {
                    Locator locator = locator_observers.Key;
                    if (!locator.HasAgent(agent)) continue;
                    List<IObserver> observers = locator_observers.Value;
                    foreach (IObserver observer in observers)
                    {
                        _SetObservation(locator, observer.GetObservation());
                    }
                }
                foreach (var locator_observations in m_Observations)
                {
                    Locator locator = locator_observations.Key;
                    if (!locator.HasAgent(agent)) continue;
                    List<IInstance> observations = locator_observations.Value;
                    foreach (IInstance observation in observations)
                    {
                        ObservationProto observationProto = new ObservationProto();
                        observationProto.Locator = locator.ToProtobuf();
                        if (observation == null) Debug.LogWarning(locator + " is null observation");
                        else observationProto.Observation = observation.ToProtobuf();
                        observationProtos.Add(observationProto);
                    }
                    observations.Clear();
                }
            }
            return observationProtos;
        }

        void _RemoveObservation(string agent)
        {
            foreach (Locator locator in m_Observations.Keys.ToList())
            {
                if (locator.HasSingleAgent(agent)) m_Observations.Remove(locator);
            }
        }

        void _AddReward(string agent, double reward)
        {
            if (!m_Rewards.ContainsKey(agent)) m_Rewards[agent] = 0;
            m_Rewards[agent] += reward;
        }

        void _SetReward(string agent, double reward)
        {
            m_Rewards[agent] = reward;
        }

        List<RewardProto> _GetRewards(List<string> responseAgents)
        {
            // TODO v: just send the requested agents
            List<RewardProto> rewardProtos = new List<RewardProto>();
            foreach (string agent in responseAgents)
            {
                if (m_Rewards.ContainsKey(agent))
                {
                    RewardProto rewardProto = new RewardProto();
                    rewardProto.Agent = agent;
                    rewardProto.Reward = m_Rewards[agent];
                    rewardProtos.Add(rewardProto);
                    m_Rewards[agent] = 0;
                }
            }
            return rewardProtos;
        }

        void _Terminate(string agent)
        {
            m_TerminatedAgents.Add(agent);
        }

        List<string> _GetTerminations(List<string> responseAgents)
        {
            // TODO v: when terminations should be included? or when should be reset? 已經停掉的也不會送東西過來了
            List<string> terminatedAgents = new List<string>();
            foreach (string agent in responseAgents)
            {
                if (m_TerminatedAgents.Contains(agent))
                {
                    terminatedAgents.Add(agent);
                    m_TerminatedAgents.RemoveAll(x => x == agent);
                }
                else if (m_TerminatedAgents.Contains("")) m_TerminatedAgents.RemoveAll(x => x == agent);
            }
            return terminatedAgents;
        }

        void _Truncate(string agent)
        {
            m_TruncationAgents.Add(agent);
        }

        List<string> _GetTruncations(List<string> responseAgents)
        {
            // TODO v: when truncations should be included? or when should be reset? 已經停掉的也不會送東西過來了
            List<string> truncatedAgents = new List<string>();
            foreach (string agent in responseAgents)
            {
                if (m_TruncationAgents.Contains(agent))
                {
                    truncatedAgents.Add(agent);
                    m_TruncationAgents.RemoveAll(x => x == agent);
                }
                else if (m_TruncationAgents.Contains("")) m_TruncationAgents.RemoveAll(x => x == agent);
            }
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

        List<InfoProto> _GetInfos(List<string> responseAgents)
        {
            // TODO v: when info should be cleared? and when env "" info be cleared?
            // TODO v: just send the requested agents
            List<InfoProto> infoProtos = new List<InfoProto>();
            foreach (string agent in responseAgents)
            {
                if (m_Infos.ContainsKey(agent))
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
            }
            return infoProtos;
        }

        void _Render(GymizeProto gymizeProto)
        {
            if (gymizeProto.Rendering != null)
            {
                foreach (ViewProto viewConfig in gymizeProto.Rendering.ViewConfigs)
                {
                    m_Render.IsSingleFrame[viewConfig.Name] = viewConfig.IsSingleFrame;
                    m_Render.ScreenWidths[viewConfig.Name] = viewConfig.ScreenWidth;
                    m_Render.ScreenHeights[viewConfig.Name] = viewConfig.ScreenHeight;
                    m_Render.IsFullscreen[viewConfig.Name] = viewConfig.Fullscreen;
                }
                foreach (string name in gymizeProto.Rendering.BeginViews)
                {
                    m_Render.Begin(name);
                }
                foreach (string name in gymizeProto.Rendering.EndViews)
                {
                    m_Render.End(name);
                }
                m_RequestViews = new List<string>(gymizeProto.Rendering.RequestViews);
            }
        }

        void _RequestAgents(GymizeProto gymizeProto)
        {
            lock (m_Requested)
            {
                foreach (string agent in gymizeProto.RequestAgents)
                {
                    if (!m_Requested.ContainsKey(agent)) m_Requested[agent] = false;
                    m_Requested[agent] = true;
                }
            }
        }

        void _SetPeriod(string agent, int period)
        {
            m_Periods[agent] = period;
        }

        void _RemovePeriod(string agent)
        {
            m_Periods.Remove(agent);
        }

        void _Tick()
        {
            _CheckChannel();

            // Control the Update rate of each agent (include env "")
            lock (m_UpdateAgents)
            {
                foreach (string agent in m_Ticks.Keys.ToList())
                {
                    m_Ticks[agent] = (m_Ticks[agent] + 1) % m_Periods[agent];
                    if (m_Ticks[agent] == 0) m_UpdateAgents[agent] = true;
                }
            }

            _SendGymizeMessage();
        }

        void _CheckChannel()
        {
            if (HasMessage("_gym_"))
            {
                Content content = TakeMessage("_gym_");
                if (content.IsBinary)
                {
                    GymizeProto gymizeProto = GymizeProto.Parser.ParseFrom(content.Raw);
                    _RecvGymizeMessage(gymizeProto);
                }
            }
            else QuitWhenChannelClosed();
        }

        void _RecvGymizeMessage(GymizeProto gymizeProto)
        {
            _Reset(gymizeProto);
            _SetActions(gymizeProto);
            _RecvInfos(gymizeProto);
            _RequestAgents(gymizeProto);
            _Render(gymizeProto);
        }

        void _SendGymizeMessage()
        {
            List<string> responseAgents = _GetResponseAgents(); // TODO v: also send this -> response_agents, add in protobuf, 救的概念，可以再繼續動了
            GymizeProto gymizeProto = new GymizeProto();
            gymizeProto.ResponseAgents.AddRange(responseAgents);
            gymizeProto.Observations.AddRange(_GetObservations(responseAgents));
            gymizeProto.Rewards.AddRange(_GetRewards(responseAgents));
            gymizeProto.TerminatedAgents.AddRange(_GetTerminations(responseAgents));
            gymizeProto.TruncatedAgents.AddRange(_GetTruncations(responseAgents));
            gymizeProto.Infos.AddRange(_GetInfos(responseAgents));
            if (responseAgents.Count > 0 || m_RequestViews.Count > 0)
            {
                if (m_RequestViews.Count > 0)
                {
                    gymizeProto.Rendering = new RenderProto();
                    gymizeProto.Rendering.Videos.AddRange(m_Render.GetRendering(m_RequestViews));
                    m_RequestViews.Clear();
                }
                byte[] gymizeMessage = gymizeProto.ToByteArray();
                TellSync("_gym_", gymizeMessage);
            }
        }

        List<string> _GetResponseAgents()
        {
            // TODO v: with m_UpdateAgents
            List<string> responseAgents = new List<string>();

            lock (m_Requested) lock (m_UpdateAgents)
            {
                foreach (string agent in m_Requested.Keys)
                {
                    if (!m_Requested.ContainsKey(agent)) continue;
                    if (!m_UpdateAgents.ContainsKey(agent)) continue;
                    if (m_Requested[agent] && m_UpdateAgents[agent]) responseAgents.Add(agent);
                }

                // remove the used requested agents
                foreach (string agent in responseAgents)
                {
                    if (!m_Requested.ContainsKey(agent)) continue;
                    if (!m_UpdateAgents.ContainsKey(agent)) continue;
                    m_Requested[agent] = false;
                    m_UpdateAgents[agent] = false;
                }
            }

            return responseAgents;
        }
    }
}