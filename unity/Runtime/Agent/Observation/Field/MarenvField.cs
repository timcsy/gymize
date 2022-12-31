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
        public FieldType Type; // SEQUENCE, TUPLE, DICT
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

    public class MarenvField
    {
        public bool IsAllAgents;
        public List<string> Agents; // If AllAgents == true, then this is the list of unavalible agents
        public bool IsRoot;
        public int Upper;
        public List<FieldPath> Paths;
        public FieldMapping Mapping; // If Mapping == null, means put all data in the last path

        public MarenvField()
        {
            IsAllAgents = false;
            Agents = new List<string>();
            IsRoot = false;
            Upper = 0;
            Paths = new List<FieldPath>();
            Mapping = null;
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

        public static MarenvField FromString(string text)
        {
            Lexer lexer = new Lexer(text);
            Parser parser = new Parser(lexer);
            Interpreter interpreter = new Interpreter(parser);
            return interpreter.Interpret();
        }
	}
}