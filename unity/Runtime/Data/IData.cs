using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public enum MarenvType
    {
        BOX,
        DISCRETE,
        MULTI_BINARY,
        MULTI_DISCRETE,
        TEXT,
        DICT,
        TUPLE,
        SEQUENCE,
        GRAPH,
        IMAGE
    }

    public interface IData
    {
        IData Merge(IData original, Mapping mapping); // mapping = null means merge the current one and the original
        Data ToProtobuf();
        void FromProtobuf(Data data);
    }
}