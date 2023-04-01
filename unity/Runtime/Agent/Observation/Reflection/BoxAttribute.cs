using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Gymize
{
    public class BoxAttribute : AttributeBase
    {
        public BoxAttribute() : base() {}
        public BoxAttribute(string location) : base(location) {}
        public BoxAttribute(List<string> locations) : base(locations) {}

        public override IData GetData(object o)
        {
            return null;
        }
    }
}