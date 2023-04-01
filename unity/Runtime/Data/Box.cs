using System;
using System.Collections;
using System.Collections.Generic;
using PAIA.Gymize.Protobuf;

namespace PAIA.Gymize
{
    // Reusing DataType same as in the PAIA.Gymize.Protobuf, so be careful
    public enum DATA_TYPE
    {
        UNSPECIFIED,
        FLOAT,
        INT
    }

    public class Box : IData
    {
        DATA_TYPE m_Type;
        List<int> m_Shape;
        List<double> m_DoubleArray;
        List<long> m_LongArray;

        public Box(DATA_TYPE type, List<int> shape = null)
        {
            m_Type = type;
            if (shape == null) shape = new List<int>();
            m_Shape = shape;
            InitializeArray(m_Type);
        }

        public Box(List<int> shape, List<double> array)
        {
            m_Type = DATA_TYPE.FLOAT;
            m_Shape = shape;
            m_DoubleArray = array;
            int dims = 1;
            foreach (int dim in m_Shape) dims *= dim;
            if (dims != m_DoubleArray.Count) throw new Exception("Wrong shape");
        }

        public Box(List<int> shape, List<long> array)
        {
            m_Type = DATA_TYPE.INT;
            m_Shape = shape;
            m_LongArray = array;
            int dims = 1;
            foreach (int dim in m_Shape) dims *= dim;
            if (dims != m_LongArray.Count) throw new Exception("Wrong shape");
        }

        void InitializeArray(DATA_TYPE type)
        {
            switch (m_Type)
            {
                case DATA_TYPE.FLOAT: m_DoubleArray = new List<double>(); break;
                case DATA_TYPE.INT: m_LongArray = new List<long>(); break;
                default: break;
            }
        }

        IEnumerable GetArray()
        {
            switch (m_Type)
            {
                case DATA_TYPE.FLOAT: return m_DoubleArray;
                case DATA_TYPE.INT: return m_LongArray;
                default: return null;
            }
        }

        public Data ToProtobuf()
        {
            Tensor tensor = new Tensor();
            tensor.Shape.Add(m_Shape);
            // tensor.FloatArray.Add(m_Array);
            Data data = new Data
            {
                SpaceType = Protobuf.SpaceType.Box,
                Box = tensor
            };
            return data;
        }
        
        public void FromProtobuf(Data data)
        {
            int[] shape = new int[data.Box.Shape.Count];
            m_Shape = new List<int>(shape);
            // float[] array = new float[data.Box.FloatArray.Count];
            // m_Array = new List<float>(array);
        }

        public IData Merge(IData original, Mapping mapping)
        {
            // TODO
            if (original == null) original = new Box(m_Type);
            Box origin = original as Box;
            if (origin != null)
            {

            }
            else Gymize.Error("Wrong data structure mapping with a Box");
            return null;
        }
    }
}