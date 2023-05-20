using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gymize.Protobuf;

namespace Gymize
{
    public class Mapping
    {
        public bool IsAllAgents;
        public List<string> Agents; // If IsAllAgents == true, then this is the list of unavalible agents
        public bool IsRoot;
        public List<Selector> Destination;
        public List<Selector> Source;

        public Mapping()
        {
            IsAllAgents = false;
            Agents = new List<string>();
            IsRoot = false;
            Destination = new List<Selector>();
            Source = new List<Selector>();
        }
        public Mapping(Match m, Dictionary<string, int> groupIndex, int end)
        {
            IsAllAgents = false;
            Agents = new List<string>();
            IsRoot = false;
            Destination = new List<Selector>();
            Source = new List<Selector>();

            CaptureCollection cc;
            Capture c;

            cc = m.Groups["root"].Captures;
            if (groupIndex["root"] < cc.Count && cc[groupIndex["root"]].Index < end)
            {
                IsRoot = true;
                groupIndex["root"]++;
            }

            cc = m.Groups["all"].Captures;
            if (groupIndex["all"] < cc.Count && cc[groupIndex["all"]].Index < end)
            {
                IsAllAgents = true;
                IsRoot = true;
                groupIndex["all"]++;
            }

            cc = m.Groups["agent"].Captures;
            while (groupIndex["agent"] < cc.Count && cc[groupIndex["agent"]].Index < end)
            {
                c = cc[groupIndex["agent"]];
                int agentEnd = c.Index + c.Length;
                Agents.Add(Atom.GetString(m, groupIndex, agentEnd));
                IsRoot = true;
                groupIndex["agent"]++;
            }

            int destinationEnd = end;
            cc = m.Groups["source"].Captures;
            if (groupIndex["source"] < cc.Count && cc[groupIndex["source"]].Index < end)
            {
                c = cc[groupIndex["source"]];
                destinationEnd = c.Index - 1;
                groupIndex["source"]++;
            }

            cc = m.Groups["selector"].Captures;
            while (groupIndex["selector"] < cc.Count && cc[groupIndex["selector"]].Index < destinationEnd)
            {
                c = cc[groupIndex["selector"]];
                int selectorEnd = c.Index + c.Length;
                Destination.Add(new Selector(m, groupIndex, selectorEnd));
                groupIndex["selector"]++;
            }

            cc = m.Groups["slices"].Captures;
            while (groupIndex["slices"] < cc.Count && cc[groupIndex["slices"]].Index < destinationEnd)
            {
                c = cc[groupIndex["slices"]];
                int arrayEnd = c.Index + c.Length;
                Destination.Add(new Selector(m, groupIndex, arrayEnd));
            }

            cc = m.Groups["selector"].Captures;
            while (groupIndex["selector"] < cc.Count && cc[groupIndex["selector"]].Index < end)
            {
                c = cc[groupIndex["selector"]];
                int selectorEnd = c.Index + c.Length;
                Source.Add(new Selector(m, groupIndex, selectorEnd));
                groupIndex["selector"]++;
            }

            cc = m.Groups["slices"].Captures;
            while (groupIndex["slices"] < cc.Count && cc[groupIndex["slices"]].Index < end)
            {
                c = cc[groupIndex["slices"]];
                int arrayEnd = c.Index + c.Length;
                Source.Add(new Selector(m, groupIndex, arrayEnd));
            }
        }

        public override string ToString()
        {
            string output = "";
            if (IsAllAgents) output += "@@";
            Agents.Sort();
            foreach (string agent in Agents) output += "\'" + agent.Replace("\'", "\\\'") + "\'@";
            if (!IsAllAgents && Agents.Count == 0 && IsRoot) output += "@";
            foreach (Selector selector in Destination) output += selector.ToString();
            output += " = $";
            foreach (Selector selector in Source) output += selector.ToString();
            return output;
        }

        public MappingProto ToProtobuf()
        {
            MappingProto mappingProto = new MappingProto();
            mappingProto.IsAllAgents = IsAllAgents;
            mappingProto.Agents.AddRange(Agents);
            mappingProto.IsRoot = IsRoot;
            foreach (Selector seletor in Destination)
            {
                mappingProto.Destination.Add(seletor.ToProtobuf());
            }
            foreach (Selector seletor in Source)
            {
                mappingProto.Source.Add(seletor.ToProtobuf());
            }
            return mappingProto;
        }

        public bool HasAgent(string agent)
        {
            if (agent == "") return IsAllAgents;
            if (Agents.Contains(agent)) return !IsAllAgents;
            else return IsAllAgents;
        }

        public bool HasSequence()
        {
            foreach (Selector selector in Destination)
            {
                if (selector.Type == SelectorType.SEQUENCE) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // Check for null and compare run-time types.
            if (this == obj) return true;
            else return ToString() == obj.ToString();
        }
    }
}