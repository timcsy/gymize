namespace Gymize
{
    public class BoxAttribute : AttributeBase
    {
        string m_DType;
        public BoxAttribute() : base()
        {
            m_DType = null;
        }
        public BoxAttribute(string locator, string dtype = null) : base(locator)
        {
            m_DType = dtype;
        }

        public override IInstance GetData(object o)
        {
            Tensor tensor = new Tensor(o, m_DType);
            return tensor;
        }
    }
}