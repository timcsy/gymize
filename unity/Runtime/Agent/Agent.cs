using System.Collections.Generic;
using UnityEngine;

namespace Gymize
{
    public abstract class Agent : MonoBehaviour, IAgent
    {
        public string Name = "agent";
        public string GetName() { return Name; }

        public abstract void OnReset();
        public abstract void OnAction(object action);
        public virtual void AddReward(double reward)
        {
            GymEnv.AddReward(GetName(), reward);
        }
        public virtual void SetReward(double reward)
        {
            GymEnv.SetReward(GetName(), reward);
        }
        public virtual void Terminate()
        {
            GymEnv.SetTermination(GetName());
        }
        public virtual void Truncate()
        {
            GymEnv.SetTruncation(GetName());
        }
        public virtual void SendInfo(object info)
        {
            GymEnv.SendInfo(GetName(), info);
        }
        public abstract void OnInfo(object info);

        public virtual void AddToEnv()
        {
            m_Observers = GymEnv.AddAgent(this);
        }

        public virtual void RemoveFromEnv()
        {
            GymEnv.RemoveAgent(this, m_Observers);
        }

        public virtual void OnEnable()
        {
            AddToEnv(); // Make sure to include this method when overriding OnEnable() !!!
        }

        public virtual void OnDisable()
        {
            RemoveFromEnv(); // Make sure to include this method when overriding OnDisable() !!!
        }

        void Start() {}

        private List<IObserver> m_Observers = new List<IObserver>();
    }
}