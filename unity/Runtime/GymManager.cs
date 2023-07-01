using System.Collections.Generic;
using UnityEngine;

namespace Gymize
{
    public class GymManager : MonoBehaviour
    {
        public string EnvName;

        // Start is called before the first frame update
        void Start()
        {
            GymEnv.Start(EnvName);
        }

        void FixedUpdate()
        {
            GymEnv.Tick();
        }

        void OnApplicationQuit()
        {
            // !!! Remember to close the GymEnv when the Unity Application is quit !!!
            GymEnv.Close();
        }
    }
}
