using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PAIA.Marenv;

public class TestAgent : MonoBehaviour, IAgent
{
    int m_intVar;

    [Box]
    public int intVarField;

    [Box(".intVarProp")]
    public int intVarProp
    {
        get { return 3; }
    }

    // [Box(".floatVarAttr")]
    // float floatVar;

    // [Box(".intArrayAttr")]
    // int[] intArray;

    // [Box(".int2ArrayAttr")]
    // int[,] int2Array;

    // [Box(".intArrayArrayAttr")]
    // int[][] intArrayArray;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
