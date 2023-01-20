using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public class BoxAttribute : AttributeBase
    {
        public BoxAttribute() : base() {}
        public BoxAttribute(string field) : base(field) {}
        public BoxAttribute(List<string> fields) : base(fields) {}
        
    }
}