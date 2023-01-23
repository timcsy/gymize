using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Marenv
{
    public abstract class SensorComponent : MonoBehaviour, ISensor
    {
        public List<string> Locations;

        // Start is called before the first frame update
        void Start() {}

        public List<string> GetLocations()
        {
            return Locations;
        }

        public abstract IData GetObservation(int cacheId = -1);
    }
}
