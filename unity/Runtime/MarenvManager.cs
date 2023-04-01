using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Marenv
{
    public class MarenvManager : MonoBehaviour
    {
        public List<SensorComponent> Sensors;
        public string Location;

        // Start is called before the first frame update
        void Start()
        {
            foreach (SensorComponent sensor in Sensors)
            {
                Marenv.AddObserver(sensor.GetLocations(), sensor);
            }
            // TestLocation();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        void FixedUpdate()
        {
            Marenv.Instance._Tick();
        }

        void TestLocation()
        {
            Debug.Log("===============Test Location===============");
            Location location = PAIA.Marenv.Location.ParseFrom(Location);
            Debug.Log(location);
        }
    }
}
