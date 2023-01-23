using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Marenv
{
    public abstract class SensorComponent : MonoBehaviour, ISensor
    {
        public List<string> Mappings;

        // Start is called before the first frame update
        void Start() {}

        public List<string> GetFields()
        {
            return Mappings;
        }

        public abstract IData GetObservation(int cacheId = -1);
    }
}
