using System;
using System.Collections;
using System.Collections.Generic;
using Gymize.Protobuf;

namespace Gymize
{
    public static class GymInstance
    {
        public static IInstance ToGym(object obj)
        {
            // From object convert to Gym instance
            if (obj == null) return null;

            IInstance instance = obj as IInstance;
            Enum e = obj as Enum;
            IDictionary dict = obj as IDictionary;
            IEnumerable enumerable = obj as IEnumerable;

            if (instance != null) return instance;
            else if (obj is string) return new Text(obj);
            else if (dict != null) return new Dict(dict);
            try
            {
                return new Scalar(obj);
            }
            catch (InvalidOperationException)
            {
                try
                {
                    return new Tensor(obj);
                }
                catch (NotImplementedException)
                {
                    if (enumerable != null) return new List(enumerable);
                    else return new Text(obj.ToString());
                }
            }
        }

        public static object ToObject(IInstance instance)
        {
            // From Gym instance convert to object
            if (instance == null) return null;
            
            Tensor tensor = instance as Tensor;
            Scalar scalar = instance as Scalar;
            Text text = instance as Text;
            Json json = instance as Json;
            Dict dict = instance as Dict;
            List list = instance as List;

            if (tensor != null) return tensor.NDArray;
            else if (scalar != null) return scalar.Value;
            else if (text != null) return text.Value;
            else if (json != null) return json.Object;
            else if (dict != null)
            {
                Dictionary<string, object> dictObj = new Dictionary<string, object>();
                foreach (KeyValuePair<string, IInstance> kvp in dict)
                {
                    dictObj.Add(kvp.Key, ToObject(kvp.Value));
                }
                return dictObj;
            }
            else if (list != null)
            {
                List<object> listObj = new List<object>();
                foreach (IInstance item in list)
                {
                    listObj.Add(ToObject(item));
                }
                return listObj;
            }
            else return instance;
        }

        public static object ParseFrom(InstanceProto proto)
        {
            // From protobuf convert to object
            switch (proto.Type)
            {
                case InstanceTypeProto.Raw:
                    return proto.RawData.ToByteArray();
                case InstanceTypeProto.Tensor:
                    return Tensor.ParseFrom(proto.Tensor);
                case InstanceTypeProto.Discrete:
                    return Scalar.ParseFrom(proto.Discrete);
                case InstanceTypeProto.Float:
                    return Scalar.ParseFrom(proto.Float);
                case InstanceTypeProto.Bool:
                    return Scalar.ParseFrom(proto.Boolean);
                case InstanceTypeProto.Null:
                    return null;
                case InstanceTypeProto.Text:
                    return Text.ParseFrom(proto.Text);
                case InstanceTypeProto.Graph:
                    return GraphInstance.ParseFrom(proto.Graph);
                case InstanceTypeProto.Dict:
                    return Dict.ParseFrom(proto.Dict);
                case InstanceTypeProto.List:
                    return List.ParseFrom(proto.List);
                default:
                    throw new NotImplementedException("");
            }
        }
    }
}