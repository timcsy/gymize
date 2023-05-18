using System;
using System.Collections;

namespace Gymize
{
    public class ListAttribute : AttributeBase
    {
        public ListAttribute() : base() {}
        public ListAttribute(string locator) : base(locator) {}

        public override IInstance GetData(object o)
        {
            if (o == null) return null;

            Type type = o.GetType();

            List instance = o as List;
            IEnumerable enumerable = o as IEnumerable;

            if (instance != null) return instance;
            else if (enumerable != null) return new List(enumerable);
            else throw new NotImplementedException();
        }
    }
}