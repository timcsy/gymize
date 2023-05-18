using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Gymize
{
    public static class Atom
    {
        public static string GetString(Match m, Dictionary<string, int> groupIndex, int end)
        {
            CaptureCollection cc;
            Capture c;

            cc = m.Groups["NAME"].Captures;
            if (groupIndex["NAME"] < cc.Count && cc[groupIndex["NAME"]].Index < end)
            {
                c = cc[groupIndex["NAME"]];
                groupIndex["NAME"]++;
                return c.Value;
            }

            cc = m.Groups["QUOTED_STRING"].Captures;
            if (groupIndex["QUOTED_STRING"] < cc.Count && cc[groupIndex["QUOTED_STRING"]].Index < end)
            {
                c = cc[groupIndex["QUOTED_STRING"]];
                groupIndex["QUOTED_STRING"]++;
                return Regex.Unescape(c.Value);
            }

            return null;
        }
    }
}