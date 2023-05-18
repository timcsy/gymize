using System;
using System.Collections;

namespace Gymize
{
    public class DictAttribute : AttributeBase
    {
        public DictAttribute() : base() {}
        public DictAttribute(string locator) : base(locator) {}

        public override IInstance GetData(object o)
        {
            if (o == null) return null;

            Dict instance = o as Dict;
            IDictionary dict = o as IDictionary;

            if (instance != null) return instance;
            else if (dict != null) return new Dict(dict);
            else throw new NotImplementedException();
        }
    }
}