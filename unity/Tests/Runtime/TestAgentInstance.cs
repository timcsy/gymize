using System;
using System.Collections;
using System.Collections.Generic;
using NumSharp;
using UnityEngine;
using Gymize;

public class TestAgentInstance : Agent
{    
    [Box]
    public int intVarField;

    int m_IntVar = 3;

    [Box(".floatVarProp", "float")]
    public int intVarProp
    {
        get { return m_IntVar; }
    }

    [Box(".intArrayAttr")]
    int[] intArray
    {
        get { return new int[]{ 4, 5, 6 }; }
    }

    [Box(".intListAttr")]
    List<int> intList
    {
        get { return new List<int>{ 1, 2, 3 }; }
    }

    [Obs(".obs1=$(2:8)")]
    List<int> obsList1
    {
        get { return new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; }
    }

    [Obs("=$(2:8)")]
    List<int> obs2
    {
        get { return new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; }
    }

    [Obs(".obsInt1 & .obsInt2")]
    int obsInt = 1314520;

    [Obs]
    float obsFloat = 3.14159f;

    [Obs]
    bool obsBool = false;

    // [Obs]
    // object obsNull = null;

    [Box(".int2ArrayAttr")]
    int[,] int2Array
    {
        get { return new int[,]{ {1, 2}, {4, 5} }; }
    }

    [Box(".intArrayArrayAttr")]
    int[][] intArrayArray
    {
        get { return new int[][]{ new int[]{2, 3}, new int[]{5, 6} }; }
    }

    [Box("@.ddd.stringField(:)")]
    string stringField = "hello";

    [Discrete]
    int discrete = 87;

    [Text]
    string textField = "hello";

    [Graph]
    GraphInstance graph;

    [Dict]
    Dictionary<string, object> dict
    {
        get 
        {
            return new Dictionary<string, object>
            {
                { "text", "value1" },
                { "a", 1 },
            };
        }
    }

    [List]
    List<object> tuple
    {
        get
        {
            return new List<object>
            {
                new int[,]{ {9, 4}, {8, 7} },
                "PAIA",
                689
            };
        }
    }

    [List]
    List<long> seq
    {
        get { return new List<long> { 1, 2, 3 }; }
    }

    [Box]
    SelectorType varEnum = SelectorType.DICT;

    [Box]
    bool varBool = true;

    [Box]
    short varInt16 = -32768;

    [Box]
    int varInt32 = -2147483648;

    [Box]
    long varInt64 = -9223372036854775808;

    [Box]
    byte varByte = 255;

    [Box]
    ushort varUInt16 = 65535;

    [Box]
    uint varUInt32 = 4294967295;

    [Box]
    ulong varUInt64 = 18446744073709551615;

    [Box]
    float varFloat32 = -1.0f / 0.0f;

    [Box]
    double varFloat64 = -1.0 / 0.0;

    [Box]
    decimal varDecimal = 3.14159265358979323846m;

    [Box]
    Vector2 varVector2 = new Vector2(2.1f, 2.2f);

    [Box]
    Vector2Int varVector2Int = new Vector2Int(21, 22);

    [Box]
    Vector3 varVector3
    {
        get { return transform.position; }
    }

    [Box]
    Vector3Int varVector3Int = new Vector3Int(31, 32, 33);

    [Box]
    Vector4 varVector4 = new Vector4(4.1f, 4.2f, 4.3f, 4.4f);

    [Box]
    Quaternion varQuaternion
    {
        get { return transform.rotation; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if ((object)intVarField is int) return;
        if ((object)obsInt is int) return;
        if ((object)obsFloat is int) return;
        if ((object)obsBool is int) return;
        if ((object)stringField is int) return;
        if ((object)discrete is int) return;
        if ((object)textField is int) return;
        if ((object)graph is int) return;
        if ((object)varEnum is int) return;
        if ((object)varBool is int) return;
        if ((object)varInt16 is int) return;
        if ((object)varInt32 is int) return;
        if ((object)varInt64 is int) return;
        if ((object)varByte is int) return;
        if ((object)varUInt16 is int) return;
        if ((object)varUInt32 is int) return;
        if ((object)varUInt64 is int) return;
        if ((object)varFloat32 is int) return;
        if ((object)varFloat64 is int) return;
        if ((object)varDecimal is int) return;
        if ((object)varVector2 is int) return;
        if ((object)varVector2Int is int) return;
        if ((object)varVector3 is int) return;
        if ((object)varVector3Int is int) return;
        if ((object)varVector4 is int) return;
        if ((object)varQuaternion is int) return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnReset() {}

    public override void OnAction(object action)
    {
        Dictionary<string, object> actions = action as Dictionary<string, object>;
        List<object> arr = actions["arr"] as List<object>;
        Debug.Log((long)arr[0]);
        Debug.Log(Convert.ToInt32(arr[0]));
        Debug.Log(arr[1]);
        Debug.Log(actions["speed"]);
        Debug.Log(actions["multi_bool"]);
        Debug.Log(actions["multi_dis"]);
        Debug.Log(actions["graph"]);
    }

    public override void OnInfo(object info)
    {
        graph = (GraphInstance)info;
    }
}
