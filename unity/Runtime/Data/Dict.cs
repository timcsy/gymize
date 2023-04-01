using System.Collections;
using System.Collections.Generic;
using PAIA.Gymize.Protobuf;

namespace PAIA.Gymize
{
    public class Dict : IComposite
    {
        Dictionary<string, IData> m_Dict;

        public Dict()
        {
            m_Dict = new Dictionary<string, IData>();
        }

        public Data ToProtobuf()
        {
            Protobuf.Data data = new Protobuf.Data
            {
                SpaceType = Protobuf.SpaceType.Dict
            };
            
            foreach (var kv in m_Dict)
            {
                data.Dict.Add(kv.Key, kv.Value.ToProtobuf());
            }
            
            return data;
        }
        
        public void FromProtobuf(Data data)
        {
            
        }

        public IData Merge(IData original, Mapping mapping)
        {
            // TODO
            if (original == null) original = new Dict();
            Dict origin = original as Dict;
            if (origin != null)
            {

            }
            else Gymize.Error("Wrong data structure mapping with a Dict");
            return null;
        }

        public IData Select(Path path)
        {
            if (path.Type == LocationType.DICT && path.Selector.Type == LocationType.KEY)
            {
                if (m_Dict.ContainsKey(path.Selector.Key)) return m_Dict[path.Selector.Key];
            }
            else Gymize.Error(path.ToString() + " should be a Dict");
            return null;
        }

        public void Set(Path path, IData data)
        {
            if (path.Type == LocationType.DICT && path.Selector.Type == LocationType.KEY)
            {
                if (!m_Dict.ContainsKey(path.Selector.Key)) m_Dict.Add(path.Selector.Key, data);
                else m_Dict[path.Selector.Key] = data;
            }
            else Gymize.Error(path.ToString() + " should be a Dict");
        }
    }
}