using PAIA.Gymize.Protobuf;

namespace PAIA.Gymize
{
    // Reusing SpaceType same as in the PAIA.Gymize.Protobuf, so be careful
    public enum SPACE_TYPE
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
        Data ToProtobuf();
        void FromProtobuf(Data data);
        IData Merge(IData original, Mapping mapping); // mapping = null means merge the current one and the original
    }
}