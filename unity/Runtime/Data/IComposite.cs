using PAIA.Marenv.Protobuf;

namespace PAIA.Marenv
{
    public interface IComposite : IData
    {
        void Set(Path path, IData data);
        IData Select(Path path);
        IData MakePath(Path path, LocationType nextType);
    }
}