using Gymize.Protobuf;

namespace Gymize
{
    public interface IInstance
    {
        InstanceProto ToProtobuf();
    }
}