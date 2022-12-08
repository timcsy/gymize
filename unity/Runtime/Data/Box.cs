using System.Collections;
using System.Collections.Generic;
using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public class Box : IData
    {
        List<int> m_Shape;
        List<float> m_Array;

        public Box(List<int> shape, List<float> array)
        {
            m_Shape = shape;
            m_Array = array;
        }

        public float this[int i]
        {
            // TODO
            // 到底 scalar 要算幾維？
            // get
            // {
            //     int Dim = 1;
            //     if (m_Shape.Count == 0) Dim = 0;
            //     else if (m_Shape.Count == 1) Dim = m_Shape[0];
            //     for (int j = 1; j < m_Shape.Count; ++j) Dim *= m_Shape[j];
            //     return new Box(m_Shape.GetRange(1, m_Shape.Count), m_Array.GetRange(Dim * i, Dim));
            // }
            set
            {
                m_Array[i] = value;
            }
        }

        public Data ToProtobuf()
        {
            FloatTensor tensor = new FloatTensor();
            tensor.Shape.Add(m_Shape);
            tensor.Array.Add(m_Array);
            Data data = new Data
            {
                SpaceType = SpaceType.Box,
                Box = tensor
            };
            return data;
        }
        
        public void FromProtobuf(Data data)
        {
            int[] shape = new int[data.Box.Shape.Count];
            m_Shape = new List<int>(shape);
            float[] array = new float[data.Box.Array.Count];
            m_Array = new List<float>(array);
        }
    }
}