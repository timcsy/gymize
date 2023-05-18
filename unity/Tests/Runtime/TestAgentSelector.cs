using System;
using System.Collections;
using System.Collections.Generic;
using NumSharp;
using UnityEngine;
using Gymize;

public class TestAgentSelector : Agent
{
    // // Test shallow
    // [Obs("@=$")]
    // int int3 = 4;

    [Obs("[A]=$[a] & [B]=$[b][0](:2,1:4) & [C]=$[b][1:] & " // Test Source
        + "[D][1:4]=$[d][3:8:2] & [E](1:4,3:8:2)=$[e](:3,:3:1) & " // Test Destination
        + "[F][]=$[f](:3,:3:1) & [F][]=$[g](:3,:3) & [F][]=$[h]")] // Test Sequence
    Dict Source = new Dict
    {
        { "a", 1 },
        {
            "b",
            new List
            {
                new int[,] { { 1, 2, 3, 4, 5 }, { 6, 7, 8, 9, 10 }, { 11, 12, 13, 14, 15 }, { 16, 17, 18, 19, 20 }, { 21, 22, 23, 24, 25 } },
                2,
                "PAIA"
            }
        },
        { "d", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
        { "e", new int[,] { { 1, 2, 3, 4}, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } } },
        { "f", new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } } },
        { "g", new int[,] { { 11, 12, 13 }, { 14, 15, 16 }, { 17, 18, 19 } } },
        { "h", new int[,] { { 21, 22, 23 }, { 24, 25, 26 }, { 27, 28, 29 } } }
    };

    [Obs("@[F][]")]
    int[,] arr1 = new int[,] { { 31, 32, 33 }, { 34, 35, 36 }, { 37, 38, 39 } };

    [Obs("@[F][]")]
    int[,] arr2 = new int[,] { { 41, 42, 43 }, { 44, 45, 46 }, { 47, 48, 49 } };

    // Start is called before the first frame update
    void Start()
    {
        // if ((object)int3 is int) return;
        if ((object)arr1 is int) return;
        if ((object)arr2 is int) return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnReset() {}

    public override void OnAction(object action)
    {
        Debug.Log((long)action);
    }

    public override void OnInfo(object info) {}
}
