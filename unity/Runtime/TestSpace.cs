using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; // For Encoding()
using WebSocketSharp;
using Google.Protobuf;
using PAIA.Marenv;
using PAIA.Marenv.Protobuf;

public class TestSpace : MonoBehaviour
{
    WebSocket ws;
    string serverAddress = "ws://localhost:8080";

    [Box("@[0]")]
    float num;

    // Start is called before the first frame update
    void Start()
    {
        ws = new WebSocket(serverAddress);
        ws.Compression = CompressionMethod.Deflate;
        ws.OnOpen += (sender, e) =>
        {
            Box box = new Box(new List<int>{2, 2}, new List<float>{1, 2, 3, 4});
            Data data = box.ToProtobuf();
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

        TestObs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestObs()
    {
        Debug.Log("TestObs");
        Agent agent = new Agent();
        Observer.CollectObservations(agent);
    }
}
