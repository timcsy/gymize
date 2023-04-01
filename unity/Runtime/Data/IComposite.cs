using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public interface IComposite : IData
    {
        IData Select(Path path);
        void Set(Path path, IData data);
    }
}