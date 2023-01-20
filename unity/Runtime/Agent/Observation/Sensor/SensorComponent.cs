using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Marenv
{
    public abstract class SensorComponent : MonoBehaviour, IObserver
    {
        public List<string> Fields;

        // Start is called before the first frame update
        void Start() {}

        public List<string> GetFields()
        {
            return Fields;
        }

        public abstract IData GetObservation(int cacheId = -1);
    }
}
