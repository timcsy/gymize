using PAIA.Gymize.Protobuf;

namespace PAIA.Gymize
{
    public interface IComposite : IData
    {
        IData Select(Path path);
        void Set(Path path, IData data);
    }
}