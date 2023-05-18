using System;

namespace Gymize
{
    public class DiscreteAttribute : AttributeBase
    {
        public DiscreteAttribute() : base() {}
        public DiscreteAttribute(string locator) : base(locator) {}

        public override IInstance GetData(object o)
        {
            return new Scalar(Convert.ToInt64(o));
        }
    }
}