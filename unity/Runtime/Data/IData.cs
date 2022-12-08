using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public interface IData
    {
        Data ToProtobuf();
        void FromProtobuf(Data data);
    }
}