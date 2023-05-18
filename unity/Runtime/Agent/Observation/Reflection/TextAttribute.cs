namespace Gymize
{
    public class TextAttribute : AttributeBase
    {
        public TextAttribute() : base() {}
        public TextAttribute(string locator) : base(locator) {}

        public override IInstance GetData(object o)
        {
            return new Text((string)o);
        }
    }
}