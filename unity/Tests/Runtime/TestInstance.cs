using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NumSharp;
using Gymize.Protobuf;
using Gymize;

// public static class InstanceExtensions
// {
//     public static InstanceProto ToProtobuf(this NDArray arr)
//     {
//         // Box box = new Box(new List<int>{2, 2}, new List<long>{1, 2, 3, 4});
//         // InstanceProto data = box.ToProtobuf();
//         // return data;
//         return null;
//     }

//     public static InstanceProto ToProtobuf(this object obj)
//     {
//         Type type = obj.GetType();
//         if (type.GetMethod("ToProtobuf") != null)
//         {
//             return (InstanceProto)type.GetMethod("ToProtobuf").Invoke(obj, null);
//         }
//         else if (type == typeof(NDArray))
//         {
//             NDArray arr = (NDArray)obj;
//             return arr.ToProtobuf();
//         }
//         else if (obj == null)
//         {
//             return null;
//         }
//         throw new Exception(type.ToString() + " should inherit IInstance class with ToProtobuf method!");
//     }
// }

public class TestInstance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // TestType();
        // TestGeneric();
        // TestNumpy();
        // TestExtension();
        TestTensor();
        TestDict();
        TestList();
        TestSpace();
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
    
    void TestGeneric()
    {
        Debug.Log("===============Test Generic===============");
        // int integer = 1;
        List<int> list = new List<int>{1,2,3};
        Dictionary<int, int> dict = new Dictionary<int, int>{ {1,2}, {3,4} };
        object obj = list;
        IEnumerable enumerable = obj as IEnumerable;
        if (enumerable != null)
        {
            Type t = enumerable.GetType();
            Debug.Log(t);
            if (t.IsGenericType)
            {
                Array arr = t.GetMethod("ToArray").Invoke(obj, null) as Array;
                Debug.Log(np.array(arr));
            }
        }
        // ValueType value = integer;
        // Debug.Log(np.array(value));
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
        // Debug.Log("===============Test Extension===============");
        // // object data = new Dict();
        // // Debug.Log(data.ToProtobuf());

        // object nd3 = np.array(new float[] {1, 2, 3});
        // Debug.Log(nd3.GetType());
        // Debug.Log(nd3.ToProtobuf());

        // Debug.Log("str".ToProtobuf());
    }

    void TestTensor()
    {
        Debug.Log("===============Test Tensor===============");
        Tensor tensor = new Tensor((byte)4);
        Debug.Log(tensor.ToProtobuf());
    }

    void TestDict()
    {
        Debug.Log("===============Test Dict===============");
        Dictionary<string, object> dict = new Dictionary<string, object>
        {
            { "d", 7 },
            { "e", 8 },
            { "f", 9 }
        };
        Debug.Log(dict.GetType());
        Dict dict1 = new Dict
        {
            { "a", new Tensor(1) },
            { "b", new Tensor(2) },
            { "c", new Tensor(3) }
        };
        Debug.Log(dict1);
        Dict dict2 = new Dict
        {
            { "a", 4 },
            { "b", 5 },
            { "c", 6 }
        };
        Debug.Log(dict2);
    }
    void TestList()
    {
        Debug.Log("===============Test List===============");
        List list1 = new List{ new Tensor(1), new Tensor(2), new Tensor(3) };
        Debug.Log(list1);
        List list2 = new List{ 4, 5, 6 };
        Debug.Log(list2);
    }
    void TestSpace()
    {
        Debug.Log("===============Test Space===============");
        IInstance instance = GymInstance.ToGym(
            new Dictionary<string, object>
            {
                { "a", 1 },
                { "b", new List<object>{ 1, 2, "hello" } },
                { "c", new int[,] { { 1, 2, 3 }, { 4, 5, 6 } } },
                { "d", new List<List<int>> { new List<int> { 1, 2, 3 }, new List<int> { 4, 5, 6 } } }
            }
        );
        Debug.Log(instance);
    }
}
