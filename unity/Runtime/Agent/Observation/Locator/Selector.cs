using System.Collections.Generic;
using System.Text.RegularExpressions;
using NumSharp;
using Gymize.Protobuf;

namespace Gymize
{
    public enum SelectorType
    {
        UNSPECIFIED,
        DICT,
        TUPLE,
        SEQUENCE,
        TENSOR
    }

    public class Selector
    {
        public SelectorType Type;
        public List<Slice> Slices;
        public string Key;

        public Selector()
        {
            Type = SelectorType.UNSPECIFIED;
            Slices = new List<Slice>();
            Key = null;
        }

        public Selector(Match m, Dictionary<string, int> groupIndex, int end)
        {
            Slices = new List<Slice>();

            CaptureCollection cc;
            Capture c;

            Key = Atom.GetString(m, groupIndex, end);
            if (Key != null)
            {
                Type = SelectorType.DICT;
            }
            else
            {
                cc = m.Groups["SLICE"].Captures;
                if (groupIndex["SLICE"] < cc.Count && cc[groupIndex["SLICE"]].Index < end)
                {
                    Type = SelectorType.TUPLE;
                    c = cc[groupIndex["SLICE"]];
                    Slices = new List<Slice>(Slice.ParseSlices(c.Value));
                    groupIndex["SLICE"]++;
                }

                cc = m.Groups["sequence"].Captures;
                if (groupIndex["sequence"] < cc.Count && cc[groupIndex["sequence"]].Index < end)
                {
                    Type = SelectorType.SEQUENCE;
                    groupIndex["sequence"]++;
                }

                cc = m.Groups["slices"].Captures;
                if (groupIndex["slices"] < cc.Count && cc[groupIndex["slices"]].Index < end)
                {
                    Type = SelectorType.TENSOR;
                    c = cc[groupIndex["slices"]];
                    Slices = new List<Slice>(Slice.ParseSlices(c.Value));
                    groupIndex["slices"]++;
                }
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case SelectorType.DICT:
                    return "[\'" + Key.Replace("\'", "\\\'") + "\']";
                case SelectorType.TUPLE:
                    return "[" + Slice.FormatSlices(Slices.ToArray()) + "]";
                case SelectorType.SEQUENCE:
                    return "[]";
                case SelectorType.TENSOR:
                    return "(" + Slice.FormatSlices(Slices.ToArray()) + ")";
                default:
                    return null;
            }
        }

        public SelectorProto ToProtobuf()
        {
            SelectorProto selectorProto = new SelectorProto();
            switch (Type)
            {
                case SelectorType.DICT:
                    selectorProto.Type = SelectorTypeProto.Dict;
                    break;
                case SelectorType.TUPLE:
                    selectorProto.Type = SelectorTypeProto.Tuple;
                    break;
                case SelectorType.SEQUENCE:
                    selectorProto.Type = SelectorTypeProto.Sequence;
                    break;
                case SelectorType.TENSOR:
                    selectorProto.Type = SelectorTypeProto.Tensor;
                    break;
                default:
                    selectorProto.Type = SelectorTypeProto.Unspecified;
                    break;
            }
            foreach (Slice slice in Slices)
            {
                SliceProto sliceProto = new SliceProto();
                sliceProto.HasStart = slice.Start != null;
                if (slice.Start != null) sliceProto.Start = slice.Start ?? 0;
                sliceProto.HasStop = slice.Stop != null;
                if (slice.Stop != null) sliceProto.Stop = slice.Stop ?? 0;
                sliceProto.Step = slice.Step;
                sliceProto.IsIndex = slice.IsIndex;
                sliceProto.IsEllipsis = slice.IsEllipsis;
                sliceProto.IsNewAxis = slice.IsNewAxis;
                selectorProto.Slices.Add(sliceProto);
            }
            if (Key != null) selectorProto.Key = Key;
            return selectorProto;
        }
    }
}