using System.Collections;
using System.Collections.Generic;
using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public class Discrete : IData
    {
        public Discrete()
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