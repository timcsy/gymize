using System.Collections;
using System.Collections.Generic;
using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public class Sequence : IData
    {
        public Sequence()
        {

        }

        public IData Merge(IData original, Mapping mapping)
        {
            // TODO
            return null;
        }

        public Data ToProtobuf()
        {
            return null;
        }
        
        public void FromProtobuf(Data data)
        {
            
        }
    }
}