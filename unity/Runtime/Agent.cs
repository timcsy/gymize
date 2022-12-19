using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PAIA.Marenv;

public class Agent : MonoBehaviour, IAgent
{
    [Box("@[0]")]
    float num;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual List<string> GetFields()
    {
        return null;
    }

    public virtual IData GetData()
    {
        return null;
    }
}
