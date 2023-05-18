using UnityEngine;

namespace Gymize
{
    public abstract class SensorComponent : MonoBehaviour, IObserver
    {
        public string Locator;
        public string GetLocator() { return Locator; }

        public abstract IInstance GetObservation();

        public virtual void AddToEnv()
        {
            GymEnv.AddObserver(GetLocator(), this);
        }

        public virtual void RemoveFromEnv()
        {
            GymEnv.RemoveObserver(this);
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
