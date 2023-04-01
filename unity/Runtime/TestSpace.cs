using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; // For Encoding()
using WebSocketSharp;
using Google.Protobuf;
using PAIA.Marenv;
using PAIA.Marenv.Protobuf;
using System.Reflection;

public class TestSpace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestWebSocket();
        // TestJoin();
        // TestEqual();
        // TestObs();
        // TestGetObservations();
        // TestType();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestWebSocket()
    {
        Debug.Log("===============Test WebSocket===============");
        WebSocket ws;
        string serverAddress = "ws://localhost:8080";
        ws = new WebSocket(serverAddress);
        ws.Compression = CompressionMethod.Deflate;
        ws.OnOpen += (sender, e) =>
        {
            // Box box = new Box(new List<int>{2, 2}, new List<long>{1, 2, 3, 4});
            // Data data = box.ToProtobuf();
            Data data = Marenv.GetObservations("kart1").ToProtobuf();
            byte[] blob = data.ToByteArray();
            ws.Send(blob);
        };
        ws.OnMessage += (sender, e) =>
        {
            if (e.IsText)
            {
                // Do something with e.Data
                Debug.Log("Message Received from "+((WebSocket)sender).Url + ", Data : "+ e.Data);
            }

            if (e.IsBinary)
            {
                // Do something with e.RawData
                Data data = Data.Parser.ParseFrom(e.RawData);
                Debug.Log(data.ToString());
            }
        };
        ws.OnError += (sender, e) =>
        {
            Debug.Log("The websocket has error: " + e.Message);
        };
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("The websocket is closing...");
        };
        ws.Connect();
    }

    void TestJoin()
    {
        Debug.Log("===============Test Join===============");
        Location loc1 = Location.ParseFrom("agent@.c.b");
        Location loc2 = Location.ParseFrom("..a");
        var locs = Location.Join(loc1, loc2);
        foreach (var loc in locs) Debug.Log(loc.ToString());
    }

    void TestEqual()
    {
        Debug.Log("===============Test Equal===============");
        Location loc1 = Location.ParseFrom(".a");
        Location loc2 = Location.ParseFrom(".a ");
        Debug.Log(Equals(loc1, loc2));
    }

    void TestObs()
    {
        Debug.Log("===============Test Obs===============");
        TestAgent agent = GetComponent<TestAgent>();
        Marenv.TestCollectObservers(agent);
    }

    void TestGetObservations()
    {
        Debug.Log("===============Test GetObservations===============");
        IData data = Marenv.GetObservations("kart1");
        Debug.Log(data.ToProtobuf());
    }

    void TestType()
    {
        Debug.Log("===============Test Type===============");
        Debug.Log(typeof(int));
        Debug.Log(typeof(int[]));
        Debug.Log(typeof(int[,]));
        Debug.Log(typeof(int[][]));
        Debug.Log(typeof(int*));
        Debug.Log(typeof(float));
        Debug.Log(typeof(double));
        Debug.Log(typeof(int));
        Debug.Log(typeof(int?));
        Debug.Log(typeof(uint));
        int[,] ints = {{1, 2}, {2, 3}};
        foreach (var n in ints)
        {
            Debug.Log(n.GetType());
        }
        int[][] ints2 = { new int[]{1, 2}, new int[]{3, 4} };
        foreach (var n in ints2)
        {
            Debug.Log(n.GetType());
        }
    }
}
