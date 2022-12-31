using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Marenv
{
    public class MarenvManager : MonoBehaviour
    {
        public List<SensorComponent> Sensors;
        List<IObserver> Observers;
        public string Field;

        // Start is called before the first frame update
        void Start()
        {
            MarenvField field = MarenvField.FromString(Field);
            Debug.Log(field);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
