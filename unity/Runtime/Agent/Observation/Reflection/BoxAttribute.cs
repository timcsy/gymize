using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public class BoxAttribute : AttributeBase
    {
        public BoxAttribute(List<string> fields = null, string agentName = null) : base(fields, agentName) {}
        public BoxAttribute(string field = null, string agentName = null) : base(field, agentName) {}
    }
}