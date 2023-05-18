namespace Gymize
{
    public class ObsAttribute : AttributeBase
    {
        public ObsAttribute() : base() {}
        public ObsAttribute(string locator) : base(locator) {}

        public override IInstance GetData(object o)
        {
            return GymInstance.ToGym(o);
        }
    }
}