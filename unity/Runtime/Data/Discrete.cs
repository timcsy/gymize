using System.Collections;
using System.Collections.Generic;
using PAIA.Gymize.Protobuf;

namespace PAIA.Gymize
{
    public class Discrete : IData
    {
        public Discrete()
        {

        }

        public Data ToProtobuf()
        {
            return null;
        }
        
        public void FromProtobuf(Data data)
        {
            
        }

        public IData Merge(IData original, Mapping mapping)
        {
            // TODO
            return null;
        }
    }
}