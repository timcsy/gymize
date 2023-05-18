using UnityEngine;

namespace Gymize
{
    public abstract class ActuatorComponent : MonoBehaviour, IActuator
    {
        public string Name;
        public string GetName() { return Name; }

        public abstract void OnAction(object action);

        public virtual void AddToEnv()
        {
            GymEnv.AddActuator(this);
        }

        public virtual void RemoveFromEnv()
        {
            GymEnv.RemoveActuator(this);
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
    }
}
