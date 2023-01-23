using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAIA.Marenv
{
    public class BoxAttribute : AttributeBase
    {
        public BoxAttribute() : base() {}
        public BoxAttribute(string field) : base(field) {}
        public BoxAttribute(List<string> fields) : base(fields) {}

        public override IData GetData(object o)
        {
            return null;
        }
    }
}