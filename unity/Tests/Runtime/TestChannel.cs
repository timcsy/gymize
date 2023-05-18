using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; // For Encoding()
using WebSocketSharp;
using Google.Protobuf;
using Gymize;
using Gymize.Protobuf;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public class TestChannel : MonoBehaviour
{
    // Start is called before the first frame update
    Channel channel = null;
    Task<Gymize.Content> response = null;
    void Start()
    {
        // TestWebSocket();
        // TestChannelConnectAndTask();
        // TestChannelRecv();
        // TestChannelSignalingPassive();
        // TestChannelSignalingActive();
        // TestChannelAskSync();
        // TestChannelMessageEventActive();
        // TestChannelAskPassiveEvent();
        // TestGymStep();
    }

    // Update is called once per frame
    void Update()
    {
        // TestChannelSignalingPassive_Update();
        // TestChannelSignalingActive_Update();
        // TestChannelAskSync_Update();
    }

    void FixedUpdate()
    {
        // TestGymStep_Update();
    }

    void OnApplicationQuit()
    {
        // !!! Remember to close the channel when the Unity Application is quit !!!
        if (channel != null)
        {
            channel.CloseSync();
            channel = null;
        }
    }

    void TestWebSocket()
    {
        // Debug.Log("===============Test WebSocket===============");
        // WebSocket ws;
        // string serverAddress = "ws://localhost:8080";
        // ws = new WebSocket(serverAddress);
        // ws.Compression = CompressionMethod.Deflate;
        // ws.OnOpen += (sender, e) =>
        // {
        //     // Box box = new Box(new List<int>{2, 2}, new List<long>{1, 2, 3, 4});
        //     // Data data = box.ToProtobuf();
        //     Data data = Gymize.GetObservations("kart1")?.ToProtobuf();
        //     byte[] blob = data?.ToByteArray();
        //     ws.Send(blob);
        // };
        // ws.OnMessage += (sender, e) =>
        // {
        //     if (e.IsText)
        //     {
        //         // Do something with e.Data
        //         Debug.Log("Message Received from "+((WebSocket)sender).Url + ", Data : "+ e.Data);
        //     }

        //     if (e.IsBinary)
        //     {
        //         // Do something with e.RawData
        //         Data data = Data.Parser.ParseFrom(e.RawData);
        //         Debug.Log(data.ToString());
        //     }
        // };
        // ws.OnError += (sender, e) =>
        // {
        //     Debug.Log("The websocket has error: " + e.Message);
        // };
        // ws.OnClose += async (sender, e) =>
        // {
        //     await Task.Run(() => Debug.Log("The websocket closed."));
        //     await Task.Delay(5000);
        //     Debug.Log("The websocket is closing...");
        // };
        // ws.Connect();
        // if (ws.ReadyState != WebSocketState.Open) Debug.Log("The websocket is unavailable...");
    }

    void TestChannelConnectAndTask()
    {
        Debug.Log("===============TestChannelConnectAndTask===============");
        Channel channel = new Channel("kart");
        channel.ConnectSync();
        channel.TellSync("jj", new Gymize.Content("Hello y"));
        // private async Task Signaling(bool isResume = false)
        // {
        //     Debug.Log("Signaling");
        //     await Task.Delay(5000);
        //     Debug.Log("after Signaling");
        // }
    }

    void TestChannelRecv()
    {
        Debug.Log("===============TestChannelRecv===============");
        Channel channel = new Channel("kart");
        MessageProto msg = new MessageProto
        {
            Header = new HeaderProto(),
            Content = new ContentProto()
        };
        msg.Header.MessageType = MessageTypeProto.Message;
        // msg.Header.Id = "id1";
        byte[] uuid = Guid.NewGuid().ToByteArray();
        msg.Header.Uuid = ByteString.CopyFrom(uuid);
        // msg.Content.Text = "data.Text";
        msg.Content.Raw = ByteString.CopyFrom(uuid);
        // channel.Recv(msg.ToByteArray());
    }

    void TestChannelSignalingPassive()
    {
        Debug.Log("===============Test Channel Signaling Passive===============");
        channel = new Channel("kart");
        channel.ConnectSync();
        channel.TellSync("agent1", "This is an Echo Bot ~~~");
    }
    void TestChannelSignalingPassive_Update()
    {
        if (channel != null && channel.HasMessage("agent1"))
        {
            Content msg = channel.TakeMessage("agent1");
            Debug.Log(msg);
        }
    }

    void TestChannelSignalingActive()
    {
        Debug.Log("===============Test Channel Signaling Active===============");
        channel = new Channel("kart", ChannelMode.ACTIVE);
        channel.ConnectSync();
    }
    void TestChannelSignalingActive_Update()
    {
        if (channel != null && channel.HasMessage("agent1"))
        {
            Content data = channel.TakeMessage("agent1");
            Debug.Log("> " + data);
            // channel.BroadcastSync(data);
            channel.TellSync("agent1", data);
        }
    }

    void TestChannelAskSync()
    {
        Debug.Log("===============Test Channel Ask Sync===============");
        channel = new Channel("kart", ChannelMode.ACTIVE);
        channel.ConnectSync();
        response = channel.AskSync("agent1", "This is an Echo Bot ~~~");
    }
    void TestChannelAskSync_Update()
    {
        if (channel != null && channel.IsRunning())
        {
            if (response != null && response.IsCompleted)
            {
                Content result = channel.TakeResponse(response);
                Debug.Log("> " + result.ToString());
                response = channel.AskSync("agent1", result.ToString());
            }
        }
    }

    void TestChannelMessageEventActive()
    {
        Debug.Log("===============Test Channel Message Event===============");
        channel = new Channel("kart", ChannelMode.ACTIVE);
        channel.OnOpen += async () =>
        {
            if (channel.Mode == ChannelMode.PASSIVE)
            {
                await channel.TellAsync("agent1", "This is an Echo Bot ~~~");
            }
        };
        channel.OnMessage["agent1"] += async (content) =>
        {
            Debug.Log("> " + content.ToString());
            if (channel.Mode == ChannelMode.ACTIVE)
            {
                await channel.TellAsync("agent1", content);
            }
            else
            {
                // passive is omitted
            }
        };
        channel.ConnectSync();
    }

    void TestChannelAskPassiveEvent()
    {
        Debug.Log("===============Test Channel Ask Passive Event===============");
        channel = new Channel("kart");
        channel.OnRequest["agent1"] += async (req) =>
        {
            print($"id: {req.Id}, uuid: {req.Uuid.ToString()}");
            print("> " + req.Content.ToString());
            await req.ReplyAsync(req.Content.Text);
            if (req.Content.Text == "exit") await channel.CloseAsync();
            else if (req.Content.Text == "pause") await channel.PauseAsync();
            else if (req.Content.Text == "update") await channel.UpdateAsync();
        };
        channel.ConnectSync();
    }

    void TestGymStep()
    {
        Debug.Log("===============Test Gym Step===============");
        channel = new Channel("kart");
        channel.ConnectSync();
    }
    void TestGymStep_Update()
    {
        if (channel != null && channel.HasMessage("agent"))
        {
            Content content = channel.TakeMessage("agent");
            Debug.Log("Received action: " + content.ToString());
            // InstanceProto instance = GymEnv.GetObservations("agent")?.ToProtobuf();
            // byte[] observation = instance?.ToByteArray();
            // channel.TellSync("agent", observation);
            // channel.TellSync(observation);
        }
        else if (channel != null && channel.Status == ChannelStatus.CLOSED)
        {
            #if UNITY_EDITOR
            // Application.Quit() does not work in the editor so
            // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
