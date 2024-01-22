using System;
using System.Collections;
using System.Collections.Generic;
using NumSharp;
using UnityEngine;
using Gymize;

public class TestAEC : Agent
{
    [Obs]
    int num = 3;

    public int stopCount = 0;
    int count = 0;

    // Start is called before the first frame update
    void Start()
    {
        if ((object)num is int) return;
    }

    // Update is called once per frame
    void Update()
    {
        AddReward(1);
    }

    public override void OnReset()
    {
        Debug.Log("Reset haha");
    }

    public override void OnAction(object action)
    {
        Debug.Log(Name);
        count++;
        if (count >= stopCount)
        {
            Terminate();
        }
    }

    public override void OnInfo(object obj)
    {
        Debug.Log(obj);
    }
}
