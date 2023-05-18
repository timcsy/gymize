using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gymize.Protobuf;

namespace Gymize
{
    public class Locator
    {
        public List<Mapping> Mappings;

        public Locator()
        {
            Mappings = new List<Mapping>();
        }
        public Locator(string text)
        {
            if (text == null) Mappings = new List<Mapping>();
            else
            {
                Dictionary<string, int> groupIndex = new Dictionary<string, int>
                {
                    { "mapping", 0 },
                    { "source", 0 },
                    { "root", 0 },
                    { "all", 0 },
                    { "agent", 0 },
                    { "selector", 0 },
                    { "SLICE", 0 },
                    { "sequence", 0 },
                    { "slices", 0 },
                    { "NAME", 0 },
                    { "QUOTED_STRING", 0 }
                };
                Match m = s_Regex.Match(text);
                if (m.Success)
                {
                    Parse(m, groupIndex);
                }
                else
                {
                    throw new Exception("Error when parsing: " + text);
                }
            }
        }

        public override string ToString()
        {
            string output = "";
            Mappings.Sort((x, y) => x.ToString().CompareTo(y.ToString()));
            for (int i = 0; i < Mappings.Count - 1; i++) output += Mappings[i].ToString() + " & ";
            if (Mappings.Count > 0) output += Mappings[Mappings.Count - 1].ToString();
            return output;
        }

        public LocatorProto ToProtobuf()
        {
            LocatorProto locatorProto = new LocatorProto();
            foreach (Mapping mapping in Mappings)
            {
                locatorProto.Mappings.Add(mapping.ToProtobuf());
            }
            return locatorProto;
        }

        private void Parse(Match m, Dictionary<string, int> groupIndex)
        {
            Mappings = new List<Mapping>();

            CaptureCollection cc;
            Capture c;

            cc = m.Groups["mapping"].Captures;
            while (groupIndex["mapping"] < cc.Count)
            {
                c = cc[groupIndex["mapping"]];
                int end = c.Index + c.Length;
                Mappings.Add(new Mapping(m, groupIndex, end));
                groupIndex["mapping"]++;
            }
        }

        private static string s_Name = "(?<NAME>[A-Za-z_][A-Za-z0-9_]*)";
        private static string s_DoubleQuotedString = @"\""(?<QUOTED_STRING>(?:[^\\\""]|\\.)*)\""";
        private static string s_SingleQuotedString = @"\'(?<QUOTED_STRING>(?:[^\\\']|\\.)*)\'";
        private static string s_QuotedString = "(?:" + s_DoubleQuotedString + ")|(?:" + s_SingleQuotedString + ")";
        private static string s_Atom = "(?:" + s_Name + ")|(?:" + s_QuotedString + ")";
        private static string s_ArraySelector = @"\((?<slices>[^\)]*)\)";
        private static string s_Selector = @"(?<selector>(?:\s*\.\s*" + s_Name + @"\s*)|(?:\[\s*(?:" + s_Atom + @"|(?<SLICE>[^\]A-Za-z_\""\']+)|(?<sequence>))\s*\]))";
        private static string s_Agents = @"\s*(?<root>\@\s*)|\s*(?:(?<all>\@\@\s*)(?:\s*(?<agent>" + s_Atom + @")\s*\@\s*)*)|(?:\s*(?<agent>" + s_Atom + @")\s*\@\s*)+\s*";
        private static string s_Mapping = @"\s*(?<mapping>(?:\s*"+ s_Agents + @"\s*)?(?:\s*" + s_Selector + @"\s*)*(?:\s*" + s_ArraySelector + @"\s*)*(?<source>\s*\=\s*\$(?:\s*" + s_Selector + @"\s*)*(?:\s*" + s_ArraySelector + @"\s*)*)?)\s*"; 
        private static string s_Expr = @"^(?:\s*" + s_Mapping + @"\s*)(?:\s*\&(?:\s*" + s_Mapping + @"\s*)\s*)*$";
        private static Regex s_Regex = new Regex(s_Expr, RegexOptions.Compiled);

        public static Locator ParseFrom(string text)
        {
            if (text == null) return null;
            return new Locator(text);
        }

        public static Locator Join(string scope, string locator, string defaultName = null)
        {
            Locator scopeLocator = new Locator(scope);
            Locator Loc = new Locator(locator);
            return Join(scopeLocator, Loc, defaultName);
        }

        public static Locator Join(Locator scope, Locator locator, string defaultName = null)
        {
            // Like a Cartesian Product: scope x locator
            if (scope == null) scope = new Locator();
            if (locator == null) locator = new Locator();
            if (defaultName != null && defaultName != "")
            {
                // Using the variable name as default name
                Mapping defaultMapping = Locator.ParseFrom("[\'" + defaultName.Replace("\'", "\\\'") + "\']").Mappings[0];
                foreach (Mapping mapping in locator.Mappings)
                {
                    if (!mapping.IsRoot && mapping.Destination.Count == 0)
                    {
                        mapping.Destination.Add(defaultMapping.Destination[0]);
                    }
                }
                if (locator.Mappings.Count == 0)
                {
                    locator.Mappings.Add(defaultMapping);
                }
            }

            Locator fullLoc = new Locator();
            foreach (Mapping mapping in locator.Mappings)
            {
                if (mapping.IsAllAgents || (mapping.IsRoot && mapping.Agents.Count > 0) || scope.Mappings.Count == 0)
                {
                    fullLoc.Mappings.Add(mapping);
                }
                else
                {
                    foreach(Mapping scopeMapping in scope.Mappings)
                    {
                        if (scopeMapping.Source.Count == 0)
                        {
                            if (mapping.IsRoot && mapping.Agents.Count == 0)
                            {
                                Mapping newMapping = new Mapping();
                                newMapping.IsAllAgents = scopeMapping.IsAllAgents;
                                newMapping.Agents = scopeMapping.Agents;
                                newMapping.IsRoot = scopeMapping.IsRoot;
                                newMapping.Destination = mapping.Destination;
                                newMapping.Source = mapping.Source;
                                fullLoc.Mappings.Add(newMapping);
                            }
                            else if (!mapping.IsRoot)
                            {
                                string merged = scopeMapping.ToString().Replace(" = $", "") + mapping.ToString();
                                fullLoc.Mappings.AddRange(Locator.ParseFrom(merged).Mappings);
                            }
                        }
                    }
                }
            }
            return fullLoc;
        }

        public bool HasAgent(string agent)
        {
            foreach (Mapping mapping in Mappings)
            {
                if (mapping.HasAgent(agent)) return true;
            }
            return false;
        }

        public bool HasSequence()
        {
            foreach (Mapping mapping in Mappings)
            {
                if (mapping.HasSequence()) return true;
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