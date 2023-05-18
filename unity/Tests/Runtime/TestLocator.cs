using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NumSharp;
using UnityEngine;
using Gymize;

public class TestLocator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestName();
        TestQuotedString();
        TestAtom();
        TestArraySelector();
        TestSelector();
        TestAgents();
        TestMapping();
        TestExpr();
        Test_Locator();
        TestJoin();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void ShowAssertion(Regex rg, Dictionary<string, bool> dataset, Action<Match> callback, bool showMatches = true)
    {
        int correctCount = 0;
        foreach (KeyValuePair<string, bool> kv in dataset)
        {
            string input = kv.Key;
            bool answer = kv.Value;

            if (rg.IsMatch(input) == answer)
            {
                if (showMatches) Debug.Log(input + " matches " + answer);
                Match m = rg.Match(input);
                if (m.Success && showMatches) callback(rg.Match(input));
                correctCount++;
            }
            else
            {
                if (showMatches) Debug.LogError(input + " = " + !answer + " does not match " + answer);
            }
        }
        if (correctCount == dataset.Count)
        {
            Debug.Log($"{correctCount}/{dataset.Count} are correct");
        }
        else
        {
            Debug.LogError($"{correctCount}/{dataset.Count} are correct");
        }
    }

    void TestName()
    {
        Debug.Log("===============Test Name===============");
        string expr = "^(?:" + s_Name + ")$";
        Regex rg = new Regex(expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { "abcd", true },
            { "AbCd", true },
            { "_yF", true },
            { "_Fy", true },
            { "A0_r", true },
            { "0a", false },
            { "0_a", false },
            { "0", false },
            { "A", true },
            { " ", false },
            { "", false },
            { "\"\"", false },
            { "\'\'", false },
        };

        Action<Match> callback = (m) => {
            Group g;
            CaptureCollection cc;
            g = m.Groups["NAME"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group NAME");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "', Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void TestQuotedString()
    {
        Debug.Log("===============Test Quoted String===============");
        string expr = "^(?:" + s_QuotedString + ")$";
        Regex rg = new Regex(expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { "\"\"", true },
            { "\" \"", true },
            { "\"A\"", true },
            { "\"\"\"", false },
            { "\"\"\"\"", false },
            { "\"\\\"\"", true },
            { "\"\\\\\"", true },
            { "\"\\n\"", true },
            { "\"\'\"", true },
            { "\"\\\'\"", true },
            { "\'\'", true },
            { "\' \'", true },
            { "\'A\'", true },
            { "\'\'\'", false },
            { "\'\'\'\'", false },
            { "\'\\\'\'", true },
            { "\'\\\\\'", true },
            { "\'\\n\'", true },
            { "\'\"\'", true },
            { "\'\\\"\'", true },
            { "\"\'", false },
            { "\"A\'", false },
            { "\'\"", false },
            { "\'A\"", false },
            { "AbCd", false },
            { "_yF", false },
            { "_Fy", false },
            { "A0_r", false },
            { "0a", false },
            { "0_a", false },
            { "0", false },
            { "A", false },
            { " ", false },
            { "", false },
        };

        Action<Match> callback = (m) => {
            Group g;
            CaptureCollection cc;
            g = m.Groups["QUOTED_STRING"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group QUOTED_STRING");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void TestAtom()
    {
        Debug.Log("===============Test Atom===============");
        string expr = "^(?:" + s_Atom + ")$";
        Regex rg = new Regex(expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { "\"\"", true },
            { "\" \"", true },
            { "\"A\"", true },
            { "\"\"\"", false },
            { "\"\"\"\"", false },
            { "\"\\\"\"", true },
            { "\"\\\\\"", true },
            { "\"\\n\"", true },
            { "\"\'\"", true },
            { "\"\\\'\"", true },
            { "\'\'", true },
            { "\' \'", true },
            { "\'A\'", true },
            { "\'\'\'", false },
            { "\'\'\'\'", false },
            { "\'\\\'\'", true },
            { "\'\\\\\'", true },
            { "\'\\n\'", true },
            { "\'\"\'", true },
            { "\'\\\"\'", true },
            { "\"\'", false },
            { "\"A\'", false },
            { "\'\"", false },
            { "\'A\"", false },
            { "AbCd", true },
            { "_yF", true },
            { "_Fy", true },
            { "A0_r", true },
            { "0a", false },
            { "0_a", false },
            { "0", false },
            { "A", true },
            { " ", false },
            { "", false },
        };

        Action<Match> callback = (Match m) => {
            Group g;
            CaptureCollection cc;
            g = m.Groups["NAME"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group NAME");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }
            g = m.Groups["QUOTED_STRING"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group QUOTED_STRING");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void TestArraySelector()
    {
        Debug.Log("===============Test ArraySelector===============");
        string expr = "^(?:" + s_ArraySelector + ")$";
        Regex rg = new Regex(expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { "()", true },
            { "( )", true },
            { "(::-1, :, 1:5)", true },
            { "())", false },
            { "(())", false },
            { "(", false },
            { ")", false },
        };

        Action<Match> callback = (m) => {
            Group g;
            CaptureCollection cc;
            g = m.Groups["slices"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group slices");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Slice[] slices = Slice.ParseSlices(c.Value);
                Debug.Log("Capture" + i + " = " + Slice.FormatSlices(slices) + " , Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void TestSelector()
    {
        Debug.Log("===============Test Selector===============");
        string expr = "^(?:" + s_Selector + ")$";
        Regex rg = new Regex(expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { ".key", true },
            { ".", false },
            { ".0", false },
            { "..", false },
            { "[key]", true },
            { "[\"key\"]", true },
            { "['key']", true }, // same as [\'key\']
            { "[\"key\']", false },
            { "[key\"]", false },
            { "[\"key]", false },
            { "[key\']", false },
            { "['key]", false },
            { "[]", true },
            { "[0]", true },
            { "[0:15]", true },
            { "", false },
            { " ", false },
            { "A", false },
            { "0", false },
            { "()", false },
            { "(0)", false },
            // test space
            { ". key", true },
            { ".key ", true },
            { ". key ", true },
            { ". k ey", false },
            { "[ key]", true },
            { "[key ]", true },
            { "[ key ]", true },
            { "[k ey]", false },
            { "[\"k ey\"]", true },
            { "[ ]", true },
            { "[ 0]", true },
            { "[0 ]", true },
            { "[ 0 ]", true },
            { "[ 15 ]", true },
        };

        Action<Match> callback = (m) => {
            Group g;
            CaptureCollection cc;

            g = m.Groups["selector"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group selector");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["NAME"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group NAME");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["QUOTED_STRING"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group QUOTED_STRING");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["SLICE"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group SLICE");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Slice slice = Slice.ParseSlices(c.Value)[0];
                Debug.Log("Capture" + i + " = " + slice + " , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["sequence"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group sequence");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void TestAgents()
    {
        Debug.Log("===============Test Agents===============");
        string expr = "^(?:" + s_Agents + ")$";
        Regex rg = new Regex(expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { "@", true },
            { "@@", true },
            { "agent@", true },
            { "\"agent 1\"@", true },
            { "'agent 2'@", true },
            { "agent1@agent2@", true },
            { "@@agent@", true },
            { "@@agent1@agent2@", true },
            { "@@\"agent 1\"@", true },
            { "@@'agent 2'@", true },
            { "", false },
            { "agent", false },
            { "agent1@agent2", false },
            { "agent 1@", false },
            { "@@agent", false },
            { "@@agent1@agent2", false },
            { "@@agent 1@", false },
            // test space
            { "@ ", true },
            { " @", true },
            { "@ @", false },
            { "@ @ ", false },
            { "@@ ", true },
            { " @@", true },
            { "agent @", true },
            { " agent @", true },
            { "agent @ ", true },
            { " agent @ agent2 @", true },
            { "@ @ agent @", false },
            { "@ @ agent 1@", false },
        };

        Action<Match> callback = (m) => {
            Group g;
            CaptureCollection cc;

            g = m.Groups["root"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group root");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["all"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group all");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["agent"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group agent");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["NAME"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group NAME");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["QUOTED_STRING"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group QUOTED_STRING");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void TestMapping()
    {
        Debug.Log("===============Test Mapping===============");
        string expr = "^(?:" + s_Mapping + ")$";
        Regex rg = new Regex(expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { "", true },
            { "@", true },
            { "agent@", true },
            { "[]", true },
            { "[a][0]", true },
            { "[a](0)", true },
            { "(0)(:)", true },
            { "(0)[0]", false },
            { "agent@[]", true },
            { "agent@[a][0]", true },
            { "agent@[a](0)", true },
            { "agent@(0)(:)", true },
            { "agent@(0)[0]", false },
            { "[]=$", true },
            { "[a][0]=$", true },
            { "[a](0)=$", true },
            { "(0)(:)=$", true },
            { "(0)[0]=$", false },
            { "@[]=$", true },
            { "@.a[0]=$", true },
            { "@[a](0)=$", true },
            { "@(0)(:)=$", true },
            { "@(0)[0]=$", false },
            { "[]=$[]", true },
            { "[]=$[a][0]", true },
            { "[]=$[a](0)", true },
            { "[]=$(0)(:)", true },
            { "[]=$(0)[0]", false },
            { "@a", false },
            // test space
            { "@ [ ] ( ) = $ [ ] ( )", true },
            { "@ [ ] ( ) = $ [ ] ( ) ", true },
            { " @ [ ] ( ) = $ [ ] ( )", true },
            { " @ [ ] [ ] ( ) ( ) = $ [ ] [ ] ( ) ( )", true },
        };

        Action<Match> callback = (m) => {
            Group g;
            CaptureCollection cc;

            g = m.Groups["mapping"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group mapping");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["source"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group source");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["root"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group root");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["all"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group all");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["agent"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group agent");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["selector"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group selector");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["SLICE"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group SLICE");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Slice slice = Slice.ParseSlices(c.Value)[0];
                Debug.Log("Capture" + i + " = " + slice + " , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["sequence"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group sequence");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["slices"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group slices");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Slice[] slices = Slice.ParseSlices(c.Value);
                Debug.Log("Capture" + i + " = " + Slice.FormatSlices(slices) + " , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["NAME"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group NAME");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["QUOTED_STRING"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group QUOTED_STRING");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void TestExpr()
    {
        Debug.Log("===============Test Expr===============");
        Regex rg = new Regex(s_Expr);

        Dictionary<string, bool> dataset = new Dictionary<string, bool>
        {
            { "", true },
            { "@", true },
            { "agent@&", true },
            { "agent@&=$", true },
            { "agent@&=&", false },
            { "&", true },
            { "&&", true },
            // test space
            { " ", true },
            { " @ ", true },
            { " agent @ & ", true },
            { " agent @ & = $ ", true },
            { " agent @ & = & ", false },
            { " & ", true },
            { " & & ", true },
            { "@@agent1@\"agent 2\"@['key'][0]=$(:,:,0)&'agent 3'@agent4@['key'][0]=$(:,:,0)&@", true },
        };

        Action<Match> callback = (m) => {
            Group g;
            CaptureCollection cc;

            g = m.Groups["mapping"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group mapping");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["source"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group source");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["root"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group root");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["all"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group all");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["agent"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group agent");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["selector"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group selector");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["SLICE"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group SLICE");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Slice slice = Slice.ParseSlices(c.Value)[0];
                Debug.Log("Capture" + i + " = " + slice + " , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["sequence"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group sequence");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Debug.Log("Capture" + i + " = '" + c + "' , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["slices"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group slices");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                Slice[] slices = Slice.ParseSlices(c.Value);
                Debug.Log("Capture" + i + " = " + Slice.FormatSlices(slices) + " , Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["NAME"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group NAME");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }

            g = m.Groups["QUOTED_STRING"];
            cc = g.Captures;
            if (cc.Count > 0) Debug.Log("Group QUOTED_STRING");
            for (int i = 0; i < cc.Count; i++)
            {
                Capture c = cc[i];
                string str = Regex.Unescape(c.Value);
                Debug.Log("Capture" + i + " = '" + str + "', Position = " + c.Index + ", Length = " + c.Length);
            }
        };
        
        // ShowAssertion(rg, dataset, callback, true);
        // ShowAssertion(rg, dataset, (m) => {}, true);
        ShowAssertion(rg, dataset, null, false);
    }

    void Test_Locator()
    {
        Debug.Log("===============Test Locator===============");
        Dictionary<string, string> dataset = new Dictionary<string, string>
        {
            {
                "@@agent1@\"agent 2\"@['key'][0]=$(:,:,0)&'agent 3'@agent4@['key'][0]=$(:,:,0)&@",
                "@ = $ & @@'agent 2'@'agent1'@['key'][0] = $(:,:,0) & 'agent 3'@'agent4'@['key'][0] = $(:,:,0)"
            },
            {
                "@@agent1@\"agent '2\"@['key'][0][]=$(:,:,0)&'agent \\'3'@'agent\n4'@[key][0:4:-1]=$(:,:,0)&@",
                "@ = $ & @@'agent \\'2'@'agent1'@['key'][0][] = $(:,:,0) & 'agent \\'3'@'agent\n4'@['key'][:4:-1] = $(:,:,0)"
            },
        };

        int correctCount = 0;
        foreach (KeyValuePair<string, string> kv in dataset)
        {
            string input = kv.Key;
            string answer = kv.Value;

            if (Locator.ParseFrom(input).ToString() == Locator.ParseFrom(answer).ToString())
            {
                if (Locator.ParseFrom(input).ToString() == answer) correctCount++;
                else
                {
                    Debug.LogWarning(Locator.ParseFrom(input).ToString());
                    Debug.LogError(input + " does not auto-encoded match " + answer);
                }
            }
            else
            {
                Debug.LogError(input + " does not match " + answer);
            }
        }
        if (correctCount == dataset.Count)
        {
            Debug.Log($"{correctCount}/{dataset.Count} are correct");
        }
        else
        {
            Debug.LogError($"{correctCount}/{dataset.Count} are correct");
        }
    }

    void TestJoin()
    {
        Debug.Log("===============Test Join===============");
        Dictionary<Locator, string> dataset = new Dictionary<Locator, string>
        {
            {
                Locator.Join("car@", ".var", null),
                "car@.var"
            },
            {
                Locator.Join("car1@", ".var=$(:)", null),
                "car1@.var=$(:)"
            },
            {
                Locator.Join("car2@", null, "var"),
                "car2@.var"
            },
            {
                Locator.Join("car3@", "", "var"),
                "car3@.var"
            },
            {
                Locator.Join("car4@", "=$(:)", "var"),
                "car4@.var=$(:)"
            },
            {
                Locator.Join("car5@", ".var1", "var2"),
                "car5@.var1"
            },
            {
                Locator.Join("car6@.var1", ".var2", null),
                "car6@.var1.var2"
            },
            {
                Locator.Join("car7@.var1 & agent@", ".var2", null),
                "car7@.var1.var2 & agent@.var2"
            },
            {
                Locator.Join("car8@.var1 & agent@", "", "var2"),
                "car8@.var1.var2 & agent@.var2"
            },
            {
                Locator.Join("car9@", "@", null),
                "car9@"
            },
            {
                Locator.Join("car10@", "@", "var"),
                "car10@"
            },
            {
                Locator.Join("car11@", "@=$", "var"),
                "car11@"
            },
        };

        int correctCount = 0;
        foreach (KeyValuePair<Locator, string> kv in dataset)
        {
            string input = kv.Key.ToString();
            string answer = Locator.ParseFrom(kv.Value).ToString();

            if (input == answer)
            {
                correctCount++;
            }
            else
            {
                Debug.LogError(input + " does not match " + answer);
            }
        }
        if (correctCount == dataset.Count)
        {
            Debug.Log($"{correctCount}/{dataset.Count} are correct");
        }
        else
        {
            Debug.LogError($"{correctCount}/{dataset.Count} are correct");
        }
    }
}