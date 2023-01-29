using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public enum LocationType
    {
        UNSPECIFIED,
        INDEX,
        KEY,
        SEQUENCE,
        TUPLE,
        DICT
    }

	public class Selector
	{
        public LocationType Type; // UNSPECIFIED, INDEX, KEY
        public int Index;
        public string Key;

        public Selector()
        {
            Type = LocationType.UNSPECIFIED;
            Index = 0;
            Key = "";
        }

        public override string ToString()
        {
            string output = "";
            if (Type == LocationType.INDEX) output += Index.ToString();
            else if (Type == LocationType.KEY)
            {
                output += "\"" + Key.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
            }
            return output;
        }
	}

    public class Slice
	{
        public LocationType Type; // UNSPECIFIED, INDEX, KEY
        public bool HasStart;
        public int StartIndex;
        public string StartKey;
        public bool HasEnd;
        public int EndIndex;
        public string EndKey;
        public int Step; // If Step == 0, it means atom with start value

        public Slice()
        {
            Type = LocationType.UNSPECIFIED;
            HasStart = false;
            StartIndex = 0;
            StartKey = "";
            HasEnd = false;
            EndIndex = 0;
            EndKey = "";
            Step = 1;
        }

        public override string ToString()
        {
            string output = "";
            if (Step == 0 && HasStart)
            {
                if (Type == LocationType.INDEX) output += StartIndex.ToString();
                else if (Type == LocationType.KEY) output += "\"" + StartKey.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
            }
            else
            {
                if (HasStart)
                {
                    if (Type == LocationType.INDEX) output += StartIndex.ToString();
                    else if (Type == LocationType.KEY) output += "\"" + StartKey.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
                }
                output += ":";
                if (HasEnd)
                {
                    if (Type == LocationType.INDEX) output += EndIndex.ToString();
                    else if (Type == LocationType.KEY) output += "\"" + EndKey.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
                }
                if (Step != 1)
                {
                    output += ":";
                    output += Step.ToString();
                }
            }
            return output;
        }
	}

    public class Path
    {
        public LocationType Type; // UNSPECIFIED, SEQUENCE, TUPLE, DICT
        public Selector Selector;

        public Path()
        {
            Type = LocationType.UNSPECIFIED;
            Selector = null;
        }

        public override string ToString()
        {
            string output = "";
            if (Type == LocationType.SEQUENCE) output += "[ " + Selector.ToString() + " ]";
            if (Type == LocationType.TUPLE) output += "( " + Selector.ToString() + " )";
            if (Type == LocationType.DICT) output += "{ " + Selector.ToString() + " }";
            return output;
        }
    }

    public class Assignment
    {
        public Slice Destination;
        public Slice Source; // If Source == null, it is just a selector (index or key)

        public Assignment()
        {
            Destination = null;
            Source = null;
        }

        public override string ToString()
        {
            string output = "";
            if (Destination != null) output += Destination.ToString();
            if (Source != null) output += "=" + Source.ToString();
            return output;
        }
    }

    public class Dimension
    {
        public List<Assignment> Assignments;

        public Dimension()
        {
            Assignments = new List<Assignment>();
        }

        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < Assignments.Count - 1; i++) output += Assignments[i].ToString() + " & ";
            if (Assignments.Count > 0) output += Assignments[Assignments.Count - 1].ToString();
            return output;
        }
    }

    public class Mapping
    {
        public LocationType Type; // UNSPECIFIED, SEQUENCE, TUPLE, DICT
        public List<Dimension> Dimensions;

        public Mapping()
        {
            Type = LocationType.UNSPECIFIED;
            Dimensions = new List<Dimension>();
        }

        public bool IsPath()
        {
            // check if the mapping is just a path
            if (Dimensions.Count == 1)
            {
                if (Dimensions[0].Assignments.Count == 1)
                {
                    if (Dimensions[0].Assignments[0].Source == null)
                    {
                        Slice destination = Dimensions[0].Assignments[0].Destination;
                        if (destination.Step == 0 && destination.HasStart)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsMapping()
        {
            foreach (Dimension dimension in Dimensions)
            {
                foreach (Assignment assignment in dimension.Assignments)
                {
                    if (assignment.Source == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override string ToString()
        {
            string output = "";
            if (Type == LocationType.SEQUENCE)
            {
                output += "[ ";
                for (int i = 0; i < Dimensions.Count - 1; i++) output += Dimensions[i].ToString() + ", ";
                if (Dimensions.Count > 0) output += Dimensions[Dimensions.Count - 1].ToString();
                output += " ]";
            }
            if (Type == LocationType.TUPLE)
            {
                output += "( ";
                for (int i = 0; i < Dimensions.Count - 1; i++) output += Dimensions[i].ToString() + ", ";
                if (Dimensions.Count > 0) output += Dimensions[Dimensions.Count - 1].ToString();
                output += " )";
            }
            if (Type == LocationType.DICT)
            {
                output += "{ ";
                if (Dimensions.Count == 1) output += Dimensions[0].ToString();
                output += " }";
            }
            return output;
        }
    }

    public class Location
    {
        public bool IsAllAgents;
        public List<string> Agents; // If AllAgents == true, then this is the list of unavalible agents
        public bool IsRoot;
        public int Upper;
        public List<Path> Paths; // If Paths has zero length, means there are no paths (exclude mapping)
        public Mapping Mapping; // If Mapping == null, means there is no mapping

        public Location()
        {
            IsAllAgents = false;
            Agents = new List<string>();
            IsRoot = false;
            Upper = 0;
            Paths = new List<Path>();
            Mapping = null;
        }

        public static List<Location> Join(List<Location> scopes, List<Location> locations, string defaultName = null)
        {
            // Like a Cartesian Product: scopes x locations
            if (scopes == null) scopes = new List<Location>();
            if (locations == null) locations = new List<Location>();
            if (locations.Count == 0 && defaultName != null)
            {
                // Using the variable name as default name
                locations.Add(Location.ParseFrom("{\"" + defaultName + "\"}"));
            }

            List<Location> fullLocs = new List<Location>();
            foreach(Location location in locations)
            {
                if (location.IsRoot || scopes.Count == 0) fullLocs.Add(location);
                else
                {
                    foreach(Location scope in scopes)
                    {
                        if (scope.Mapping == null)
                        {
                            string merged = scope.ToString() + location.ToString();
                            fullLocs.Add(Location.ParseFrom(merged));
                        }
                    }
                }
            }
            return fullLocs;
        }

        public static List<Location> Join(Location scope, List<Location> locations, string defaultName = null)
        {
            List<Location> scopes = null;
            if (scope != null) scopes = new List<Location>{ scope };
            return Join(scopes, locations, defaultName);
        }

        public static List<Location> Join(List<Location> scopes, Location location, string defaultName = null)
        {
            List<Location> locations = null;
            if (location != null) locations = new List<Location>{ location };
            return Join(scopes, locations, defaultName);
        }

        public static List<Location> Join(Location scope, Location location, string defaultName = null)
        {
            List<Location> scopes = null;
            if (scope != null) scopes = new List<Location>{ scope };
            List<Location> locations = null;
            if (location != null) locations = new List<Location>{ location };
            return Join(scopes, locations, defaultName);
        }

        public static Location ParseFrom(string text)
        {
            Lexer lexer = new Lexer(text);
            Parser parser = new Parser(lexer);
            Interpreter interpreter = new Interpreter(parser);
            return interpreter.Interpret();
        }

        public static List<Location> ParseFrom(List<string> locations)
        {
            List<Location> locs = new List<Location>();
            if (locations != null)
            {
                foreach (string location in locations)
                {
                    locs.Add(Location.ParseFrom(location));
                }
            }
            return locs;
        }

        public override string ToString()
        {
            string output = "";
            if (IsAllAgents) output += "@@";
            foreach (string agent in Agents) output += agent + "@";
            if (!IsAllAgents && Agents.Count == 0 && IsRoot) output += "@";
            for (int i = 0; i < Upper; i++) output += ".";
            foreach (Path path in Paths) output += path.ToString();
            if (Mapping != null) output += Mapping.ToString();
            return output;
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

        public bool HasAgent(string agent)
        {
            foreach (string a in Agents)
            {
                if (a == agent) return !IsAllAgents;
            }
            return IsAllAgents;
        }
	}
}