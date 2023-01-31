using System.Collections;
using System.Collections.Generic;
using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public class Dict : IComposite
    {
        Dictionary<string, IData> m_Dict;

        public Dict()
        {
            m_Dict = new Dictionary<string, IData>();
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

        public void Set(Path path, IData data)
        {

        }

        public IData Select(Path path)
        {
            if (path.Type == LocationType.DICT && path.Selector.Type == LocationType.KEY)
            {
                if (m_Dict.ContainsKey(path.Selector.Key)) return m_Dict[path.Selector.Key];
            }
            else Marenv.Error(path.ToString() + " should be a Dict");
            return null;
        }

        public IData MakePath(Path path, LocationType nextType)
        {
            if (path.Type == LocationType.DICT && path.Selector.Type == LocationType.KEY)
            {
                IData newData = null;
                switch (nextType)
                {
                    case LocationType.DICT:
                        newData = new Dict();
                        break;
                    case LocationType.TUPLE:
                        newData = new Tuple();
                        break;
                    case LocationType.SEQUENCE:
                        newData = new Sequence();
                        break;
                    default:
                        break;
                }
                if (!m_Dict.ContainsKey(path.Selector.Key)) m_Dict.Add(path.Selector.Key, newData);
                else if (m_Dict[path.Selector.Key] == null) m_Dict[path.Selector.Key] = newData;
                return m_Dict[path.Selector.Key];
            }
            else Marenv.Error(path.ToString() + " should be a Dict");
            return null;
        }
    }
}