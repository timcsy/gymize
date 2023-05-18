using System;
using System.Collections;
using System.Collections.Generic;
using System.Buffers.Binary;
using UnityEngine;
using NumSharp;
using NumSharp.Utilities;
using Google.Protobuf;
using Gymize.Protobuf;

namespace Gymize
{
    public class Tensor : IInstance
    {
        private NDArray m_NDArray;
        public NDArray NDArray
        {
            get { return m_NDArray; }
            set { m_NDArray = value; }
        }

        public Tensor()
        {
            m_NDArray = null;
        }
        public Tensor(object obj, string dtype = null)
        {
            Type type = obj.GetType();
            m_NDArray = null;

            Array arr = obj as Array;
            IEnumerable enumerable = obj as IEnumerable;
            Enum e = obj as Enum;

            if (type == typeof(Tensor)) m_NDArray = (obj as Tensor).NDArray;
            else if (type == typeof(NDArray)) m_NDArray = obj as NDArray;
            else if (type == typeof(string)) m_NDArray = np.array((string)obj);
            else if (arr != null) m_NDArray = np.array(arr);
            else if (enumerable != null)
            {
                if (type.IsGenericType)
                {
                    Type GType = type.GetGenericArguments()[0];
                    if (GType == typeof(bool)
                        || GType == typeof(short)
                        || GType == typeof(int)
                        || GType == typeof(long)
                        || GType == typeof(char) // not support byte, ushort
                        || GType == typeof(uint)
                        || GType == typeof(ulong)
                        || GType == typeof(float)
                        || GType == typeof(double)
                        || GType == typeof(decimal))
                    {
                        arr = type.GetMethod("ToArray").Invoke(obj, null) as Array;
                        m_NDArray = np.array(arr);
                    }
                    else throw new NotImplementedException();
                }
            }
            else if (e != null) m_NDArray = np.array(Convert.ToInt64(obj));
            else if (type == typeof(bool)) m_NDArray = np.array((bool)obj);
            else if (type == typeof(short)) m_NDArray = np.array((short)obj);
            else if (type == typeof(int)) m_NDArray = np.array((int)obj);
            else if (type == typeof(long)) m_NDArray = np.array((long)obj);
            else if (type == typeof(byte)) m_NDArray = np.array((char)(byte)obj);
            else if (type == typeof(ushort)) m_NDArray = np.array((char)(ushort)obj);
            else if (type == typeof(char)) m_NDArray = np.array((char)obj);
            else if (type == typeof(uint)) m_NDArray = np.array((uint)obj);
            else if (type == typeof(ulong)) m_NDArray = np.array((ulong)obj);
            else if (type == typeof(float)) m_NDArray = np.array((float)obj);
            else if (type == typeof(double)) m_NDArray = np.array((double)obj);
            else if (type == typeof(decimal)) m_NDArray = np.array((decimal)obj);
            else if (type == typeof(Vector2))
            {
                Vector2 v = (Vector2)obj;
                m_NDArray = np.array(new float[]{ v.x, v.y });
            }
            else if (type == typeof(Vector2Int))
            {
                Vector2Int v = (Vector2Int)obj;
                m_NDArray = np.array(new int[]{ v.x, v.y });
            }
            else if (type == typeof(Vector3))
            {
                Vector3 v = (Vector3)obj;
                m_NDArray = np.array(new float[]{ v.x, v.y, v.z });
            }
            else if (type == typeof(Vector3Int))
            {
                Vector3Int v = (Vector3Int)obj;
                m_NDArray = np.array(new int[]{ v.x, v.y, v.z });
            }
            else if (type == typeof(Vector4))
            {
                Vector4 v = (Vector4)obj;
                m_NDArray = np.array(new float[]{ v.w, v.x, v.y, v.z });
            }
            else if (type == typeof(Quaternion))
            {
                Quaternion v = (Quaternion)obj;
                m_NDArray = np.array(new float[]{ v.w, v.x, v.y, v.z });
            }
            else
            {
                throw new NotImplementedException("");
            }

            if (dtype != null)
            {
                m_NDArray = m_NDArray?.astype(np.dtype(dtype).typecode, false);
            }
            
            if (type == typeof(byte)) m_NDArray = m_NDArray?.astype(np.uint8, false);
            else if (m_NDArray?.dtype == typeof(char)) m_NDArray = m_NDArray.astype(np.uint16, false);
            else if (m_NDArray?.dtype == typeof(decimal)) m_NDArray = m_NDArray.astype(np.float64, false);
        }

        public NDArray this[string slice]
        {
            get => m_NDArray[slice];
            set { m_NDArray[slice] = value; }
        }

        public NDArray this[params Slice[] slice]
        {
            get => m_NDArray[slice];
            set { m_NDArray[slice] = value; }
        }

        public NDArray this[params object[] indicesObjects]
        {
            get => m_NDArray[indicesObjects];
            set { m_NDArray[indicesObjects] = value; }
        }

        public InstanceProto ToProtobuf()
        {
            if (m_NDArray == null) return null;

            TensorProto tensorProto = new TensorProto
            {
                Data = ByteString.CopyFrom(m_NDArray.ToByteArray()),
                Dtype = GetDtype()
            };
            tensorProto.Shape.AddRange(m_NDArray.shape);

            InstanceProto instance = new InstanceProto
            {
                Type = InstanceTypeProto.Tensor,
                Tensor = tensorProto
            };
            return instance;
        }

        public static object ParseFrom(TensorProto tensorProto)
        {
            // Convert to NDArray
            byte[] bytes = tensorProto.Data.ToByteArray();

            DType dtype = np.dtype(tensorProto.Dtype);
            Type type = dtype.type;
            bool reverse = tensorProto.Dtype.Length > 0 && tensorProto.Dtype[0] == '>';

            NDArray nd = null;

            if (type == typeof(bool))
            {
                nd = new NDArray(bytes);
            }
            else if (type == typeof(short))
            {
                var size = bytes.Length / InfoOf<short>.Size;
                var arr = new short[size];
                for (var index = 0; index < size; index++)
                {
                    arr[index] = BitConverter.ToInt16(bytes, index * InfoOf<short>.Size);
                    if (reverse) arr[index] = BinaryPrimitives.ReverseEndianness(arr[index]);
                }
                nd = new NDArray(arr);
            }
            else if (type == typeof(int))
            {
                var size = bytes.Length / InfoOf<int>.Size;
                var arr = new int[size];
                for (var index = 0; index < size; index++)
                {
                    arr[index] = BitConverter.ToInt32(bytes, index * InfoOf<int>.Size);
                    if (reverse) arr[index] = BinaryPrimitives.ReverseEndianness(arr[index]);
                }
                nd = new NDArray(arr);
            }
            else if (type == typeof(long))
            {
                var size = bytes.Length / InfoOf<long>.Size;
                var arr = new long[size];
                for (var index = 0; index < size; index++)
                {
                    arr[index] = BitConverter.ToInt64(bytes, index * InfoOf<long>.Size);
                    if (reverse) arr[index] = BinaryPrimitives.ReverseEndianness(arr[index]);
                }
                nd = new NDArray(arr);
            }
            else if (tensorProto.Dtype == "|u1" || tensorProto.Dtype == "u1" || tensorProto.Dtype == "=u1" || tensorProto.Dtype == "B" || tensorProto.Dtype == "|B" || tensorProto.Dtype == "=B" || type == typeof(byte))
            {
                nd = new NDArray(bytes);
            }
            else if (type == typeof(ushort))
            {
                var size = bytes.Length / InfoOf<ushort>.Size;
                var arr = new ushort[size];
                for (var index = 0; index < size; index++)
                {
                    arr[index] = BitConverter.ToUInt16(bytes, index * InfoOf<ushort>.Size);
                    if (reverse) arr[index] = BinaryPrimitives.ReverseEndianness(arr[index]);
                }
                nd = new NDArray(arr);
            }
            else if (type == typeof(uint))
            {
                var size = bytes.Length / InfoOf<uint>.Size;
                var arr = new uint[size];
                for (var index = 0; index < size; index++)
                {
                    arr[index] = BitConverter.ToUInt32(bytes, index * InfoOf<uint>.Size);
                    if (reverse) arr[index] = BinaryPrimitives.ReverseEndianness(arr[index]);
                }
                nd = new NDArray(arr);
            }
            else if (type == typeof(ulong))
            {
                var size = bytes.Length / InfoOf<ulong>.Size;
                var arr = new ulong[size];
                for (var index = 0; index < size; index++)
                {
                    arr[index] = BitConverter.ToUInt64(bytes, index * InfoOf<ulong>.Size);
                    if (reverse) arr[index] = BinaryPrimitives.ReverseEndianness(arr[index]);
                }
                nd = new NDArray(arr);
            }
            else if (type == typeof(float))
            {
                var size = bytes.Length / InfoOf<float>.Size;
                var arr = new float[size];
                for (var index = 0; index < size; index++)
                {
                    if (reverse)
                    {
                        uint temp = BitConverter.ToUInt32(bytes, index * InfoOf<float>.Size);
                        temp = BinaryPrimitives.ReverseEndianness(temp);
                        arr[index] = BitConverter.ToSingle(BitConverter.GetBytes(temp), 0);
                    }
                    else
                    {
                        arr[index] = BitConverter.ToSingle(bytes, index * InfoOf<float>.Size);
                    }
                }
                nd = new NDArray(arr);
            }
            else if (type == typeof(double))
            {
                var size = bytes.Length / InfoOf<double>.Size;
                var arr = new double[size];
                for (var index = 0; index < size; index++)
                {
                    if (reverse)
                    {
                        ulong temp = BitConverter.ToUInt64(bytes, index * InfoOf<double>.Size);
                        temp = BinaryPrimitives.ReverseEndianness(temp);
                        arr[index] = BitConverter.ToDouble(BitConverter.GetBytes(temp), 0);
                    }
                    else
                    {
                        arr[index] = BitConverter.ToDouble(bytes, index * InfoOf<double>.Size);
                    }
                }
                nd = new NDArray(arr);
            }
            else
            {
                throw new NotSupportedException();
            }

            Shape shape = new Shape(new List<int>(tensorProto.Shape).ToArray());
            return nd.reshape(shape).astype(dtype.typecode, false);
        }

        public override string ToString()
        {
            return m_NDArray?.ToString();
        }

        private string GetDtype()
        {
            Type type = m_NDArray.dtype;
            char endian = BitConverter.IsLittleEndian? '<' : '>';

            if (type == typeof(bool))
                return "?";
            if (type == typeof(short))
                return endian + "i2";
            if (type == typeof(int))
                return endian + "i4";
            if (type == typeof(long))
                return endian + "i8";
            if (type == typeof(byte))
                return "B";
            if (type == typeof(ushort))
                return endian + "u2";
            if (type == typeof(uint))
                return endian + "u4";
            if (type == typeof(ulong))
                return endian + "u8";
            if (type == typeof(float))
                return endian + "f4";
            if (type == typeof(double))
                return endian + "f8";

            throw new NotSupportedException();
        }
    }
}