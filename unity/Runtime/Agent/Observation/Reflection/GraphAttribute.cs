namespace Gymize
{
    public class GraphAttribute : AttributeBase
    {
        public GraphAttribute() : base() {}
        public GraphAttribute(string locator) : base(locator) {}

        public override IInstance GetData(object o)
        {
            GraphInstance graph = o as GraphInstance;
            return graph;
        }
    }
}