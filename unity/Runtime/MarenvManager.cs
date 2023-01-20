using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Marenv
{
    public class MarenvManager : MonoBehaviour
    {
        public List<SensorComponent> Sensors;
        public string Field;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("===============TestField===============");
            FieldString field = FieldString.ParseFrom(Field);
            Debug.Log(field);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        
        void FixedUpdate()
        {
            Marenv.Instance.Tick();
        }
    }
}
