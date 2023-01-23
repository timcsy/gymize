using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public enum FieldType
    {
        UNSPECIFIED,
        INDEX,
        KEY,
        SEQUENCE,
        TUPLE,
        DICT
    }

	public class FieldPosition
	{
        public FieldType Type; // UNSPECIFIED, INDEX, KEY
        public int Index;
        public string Key;

        public FieldPosition()
        {
            Type = FieldType.UNSPECIFIED;
            Index = 0;
            Key = "";
        }

        public override string ToString()
        {
            string output = "";
            if (Type == FieldType.INDEX) output += Index.ToString();
            else if (Type == FieldType.KEY)
            {
                output += "\"" + Key.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
            }
            return output;
        }
	}

    public class FieldSlice
	{
        public FieldType Type; // UNSPECIFIED, INDEX, KEY
        public bool HasStart;
        public int StartIndex;
        public string StartKey;
        public bool HasEnd;
        public int EndIndex;
        public string EndKey;
        public int Step; // If Step == 0, it means atom with start value

        public FieldSlice()
        {
            Type = FieldType.UNSPECIFIED;
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
                if (Type == FieldType.INDEX) output += StartIndex.ToString();
                else if (Type == FieldType.KEY) output += "\"" + StartKey.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
            }
            else
            {
                if (HasStart)
                {
                    if (Type == FieldType.INDEX) output += StartIndex.ToString();
                    else if (Type == FieldType.KEY) output += "\"" + StartKey.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
                }
                output += ":";
                if (HasEnd)
                {
                    if (Type == FieldType.INDEX) output += EndIndex.ToString();
                    else if (Type == FieldType.KEY) output += "\"" + EndKey.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\'", "\\\'") + "\"";
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

    public class FieldPath
    {
        public FieldType Type; // UNSPECIFIED, SEQUENCE, TUPLE, DICT
        public FieldPosition Position;

        public FieldPath()
        {
            Type = FieldType.UNSPECIFIED;
            Position = null;
        }

        public override string ToString()
        {
            string output = "";
            if (Type == FieldType.SEQUENCE) output += "[ " + Position.ToString() + " ]";
            if (Type == FieldType.TUPLE) output += "( " + Position.ToString() + " )";
            if (Type == FieldType.DICT) output += "{ " + Position.ToString() + " }";
            return output;
        }
    }

    public class FieldAssignment
    {
        public FieldSlice Destination;
        public FieldSlice Source; // If Source == null, it will use the default mapping

        public FieldAssignment()
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

    public class FieldCollection
    {
        public List<FieldAssignment> Assignments;

        public FieldCollection()
        {
            Assignments = new List<FieldAssignment>();
        }

        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < Assignments.Count - 1; i++) output += Assignments[i].ToString() + " & ";
            if (Assignments.Count > 0) output += Assignments[Assignments.Count - 1].ToString();
            return output;
        }
    }

    public class FieldMapping
    {
        public FieldType Type; // UNSPECIFIED, SEQUENCE, TUPLE, DICT
        public List<FieldCollection> Dimensions;

        public FieldMapping()
        {
            Type = FieldType.UNSPECIFIED;
            Dimensions = new List<FieldCollection>();
        }

        void Error(string reason)
        {
            throw new Exception("Path Error: " + reason + "\nIn field mapping: " + ToString());
        }

        public bool IsSingle()
        {
            if (Dimensions.Count == 1)
            {
                if (Dimensions[0].Assignments.Count == 1)
                {
                    if (Dimensions[0].Assignments[0].Source == null)
                    {
                        FieldSlice destination = Dimensions[0].Assignments[0].Destination;
                        if (destination.Step == 0 && destination.HasStart)
                        {
                            FieldPath path = new FieldPath();
                            path.Type = Type;
                            path.Position = new FieldPosition();
                            path.Position.Type = destination.Type;
                            path.Position.Index = destination.StartIndex;
                            path.Position.Key = destination.StartKey;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            string output = "";
            if (Type == FieldType.SEQUENCE)
            {
                output += "[ ";
                for (int i = 0; i < Dimensions.Count - 1; i++) output += Dimensions[i].ToString() + ", ";
                if (Dimensions.Count > 0) output += Dimensions[Dimensions.Count - 1].ToString();
                output += " ]";
            }
            if (Type == FieldType.TUPLE)
            {
                output += "( ";
                for (int i = 0; i < Dimensions.Count - 1; i++) output += Dimensions[i].ToString() + ", ";
                if (Dimensions.Count > 0) output += Dimensions[Dimensions.Count - 1].ToString();
                output += " )";
            }
            if (Type == FieldType.DICT)
            {
                output += "{ ";
                if (Dimensions.Count == 1) output += Dimensions[0].ToString();
                output += " }";
            }
            return output;
        }
    }

    public class FieldString
    {
        public bool IsAllAgents;
        public List<string> Agents; // If AllAgents == true, then this is the list of unavalible agents
        public bool IsRoot;
        public int Upper;
        public List<FieldPath> Paths;
        public FieldMapping Mapping; // If Mapping == null, means there is no path and mapping

        public FieldString()
        {
            IsAllAgents = false;
            Agents = new List<string>();
            IsRoot = false;
            Upper = 0;
            Paths = new List<FieldPath>();
            Mapping = null;
        }

        public static List<FieldString> Join(List<FieldString> scopes, List<FieldString> fields, string defaultName = null)
        {
            // Like a Cartesian Product: scopes x fields
            if (scopes == null) scopes = new List<FieldString>();
            if (fields == null) fields = new List<FieldString>();
            if (fields.Count == 0 && defaultName != null)
            {
                // Using the variable name as default name
                fields.Add(FieldString.ParseFrom("{\"" + defaultName + "\"}"));
            }

            List<FieldString> fullFields = new List<FieldString>();
            foreach(FieldString field in fields)
            {
                if (field.IsRoot || scopes.Count == 0) fullFields.Add(field);
                else
                {
                    foreach(FieldString scope in scopes)
                    {
                        if (scope.Mapping == null || (scope.Mapping != null && scope.Mapping.IsSingle()))
                        {
                            string merged = scope.ToString() + field.ToString();
                            fullFields.Add(FieldString.ParseFrom(merged));
                        }
                    }
                }
            }
            return fullFields;
        }

        public static List<FieldString> Join(FieldString scope, List<FieldString> fields, string defaultName = null)
        {
            List<FieldString> scopes = null;
            if (scope != null) scopes = new List<FieldString>{ scope };
            return Join(scopes, fields, defaultName);
        }

        public static List<FieldString> Join(List<FieldString> scopes, FieldString field, string defaultName = null)
        {
            List<FieldString> fields = null;
            if (field != null) fields = new List<FieldString>{ field };
            return Join(scopes, fields, defaultName);
        }

        public static List<FieldString> Join(FieldString scope, FieldString field, string defaultName = null)
        {
            List<FieldString> scopes = null;
            if (scope != null) scopes = new List<FieldString>{ scope };
            List<FieldString> fields = null;
            if (field != null) fields = new List<FieldString>{ field };
            return Join(scopes, fields, defaultName);
        }

        public static FieldString ParseFrom(string text)
        {
            Lexer lexer = new Lexer(text);
            Parser parser = new Parser(lexer);
            Interpreter interpreter = new Interpreter(parser);
            return interpreter.Interpret();
        }

        public static List<FieldString> ParseFrom(List<string> fields)
        {
            List<FieldString> fieldStrings = new List<FieldString>();
            if (fields != null)
            {
                foreach (string field in fields)
                {
                    fieldStrings.Add(FieldString.ParseFrom(field));
                }
            }
            return fieldStrings;
        }

        public override string ToString()
        {
            string output = "";
            if (IsAllAgents) output += "@@";
            foreach (string agent in Agents) output += agent + "@";
            if (!IsAllAgents && Agents.Count == 0 && IsRoot) output += "@";
            for (int i = 0; i < Upper; i++) output += ".";
            foreach (FieldPath path in Paths) output += path.ToString();
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