using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; // For Encoding()
using WebSocketSharp;
using Google.Protobuf;
using PAIA.Gymize;
using PAIA.Gymize.Protobuf;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NumSharp;

public static class InstanceExtensions
{
    public static Data ToProtobuf(this NDArray arr)
    {
        Box box = new Box(new List<int>{2, 2}, new List<long>{1, 2, 3, 4});
        Data data = box.ToProtobuf();
        return data;
    }

    public static Data ToProtobuf(this object obj)
    {
        Type type = obj.GetType();
        if (type.GetMethod("ToProtobuf") != null)
        {
            return (Data)type.GetMethod("ToProtobuf").Invoke(obj, null);
        }
        else if (type == typeof(NDArray))
        {
            NDArray arr = (NDArray)obj;
            return arr.ToProtobuf();
        }
        else if (obj == null)
        {
            return null;
        }
        throw new Exception(type.ToString() + " should inherit IInstance class with ToProtobuf method!");
    }
}

public class TestSpace : MonoBehaviour
{
    // Start is called before the first frame update
    Channel channel = null;
    Task<PAIA.Gymize.Content> response = null;
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
        // TestJoin();
        // TestEqual();
        // TestObs();
        // TestGetObservations();
        // TestType();
        TestNumpy();
        TestExtension();
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
        Debug.Log("===============Test WebSocket===============");
        WebSocket ws;
        string serverAddress = "ws://localhost:8080";
        ws = new WebSocket(serverAddress);
        ws.Compression = CompressionMethod.Deflate;
        ws.OnOpen += (sender, e) =>
        {
            // Box box = new Box(new List<int>{2, 2}, new List<long>{1, 2, 3, 4});
            // Data data = box.ToProtobuf();
            Data data = Gymize.GetObservations("kart1")?.ToProtobuf();
            byte[] blob = data?.ToByteArray();
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
        ws.OnClose += async (sender, e) =>
        {
            await Task.Run(() => Debug.Log("The websocket closed."));
            await Task.Delay(5000);
            Debug.Log("The websocket is closing...");
        };
        ws.Connect();
        if (ws.ReadyState != WebSocketState.Open) Debug.Log("The websocket is unavailable...");
    }

    void TestChannelConnectAndTask()
    {
        Debug.Log("===============TestChannelConnectAndTask===============");
        Channel channel = new Channel("kart");
        channel.ConnectSync();
        channel.TellSync("jj", new PAIA.Gymize.Content("Hello y"));
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
        PAIA.Gymize.Protobuf.Message msg = new PAIA.Gymize.Protobuf.Message
        {
            Header = new PAIA.Gymize.Protobuf.Header(),
            Content = new PAIA.Gymize.Protobuf.Content()
        };
        msg.Header.MessageType = PAIA.Gymize.Protobuf.MessageType.Message;
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
            PAIA.Gymize.Content msg = channel.TakeMessage("agent1");
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
            PAIA.Gymize.Content data = channel.TakeMessage("agent1");
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
                PAIA.Gymize.Content result = channel.TakeResponse(response);
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
        channel.OnMessage["agent1"].Event += async (content) =>
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
        channel.OnRequest["agent1"].Event += async (req) =>
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
        Gymize.TestCollectObservers(agent);
    }

    void TestGetObservations()
    {
        Debug.Log("===============Test GetObservations===============");
        IData data = Gymize.GetObservations("kart1");
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

    void TestNumpy()
    {
        Debug.Log("===============Test Numpy===============");
        var nd = np.full(5, 12);
        var nd2 = np.full(3, 12).reshape(2, 3, 2);
        nd = nd.reshape(2, 3, 2);
        var data = nd[":, 0, :"];
        nd[":, 0, :"] = nd2[":, 0, :"];
        Debug.Log(nd);
        Debug.Log(data);

        var x1 = np.array(new float[] {10, 20, 30});
        Debug.Log("shape of x1 is " + x1.shape);
        Debug.Log(x1);

        var x2 = x1[":, np.newaxis"];
        Debug.Log("shape of x2 is " + x2.shape);
        Debug.Log(x2);

        var x3 = x1["np.newaxis, :"];
        Debug.Log("shape of x3 is " + x3.shape);
        Debug.Log(x3);

        var nd3 = np.array(new float[] {1, 2, 3});
        nd3["2"] = 4;
        Debug.Log(nd3);

        var nd4 = np.array<long>();
        nd4 = 4;
        Debug.Log(nd4.shape.Length); // 0, shape = []

        // var nd5 = np.array("Hello".ToCharArray());
        // Debug.Log(nd5["2:4"]);

        var slice = new NumSharp.Slice("10:-1").ToSliceDef(50);
        Debug.Log(slice);
    }

    void TestExtension()
    {
        Debug.Log("===============Test Extension===============");
        object data = new Dict();
        Debug.Log(data.ToProtobuf());

        object nd3 = np.array(new float[] {1, 2, 3});
        Debug.Log(nd3.GetType());
        Debug.Log(nd3.ToProtobuf());

        Debug.Log("str".ToProtobuf());
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
            PAIA.Gymize.Content content = channel.TakeMessage("agent");
            Debug.Log("Received action: " + content.ToString());
            Data data = Gymize.GetObservations("agent")?.ToProtobuf();
            byte[] observation = data?.ToByteArray();
            channel.TellSync("agent", observation);
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
