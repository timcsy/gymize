using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Gymize
{
    public enum NodeType
    {
        UNSPECIFIED,
        NAME,
        STRING,
        INTEGER,
        DICT,
        TUPLE,
        SEQUENCE
    }
    public class AtomNode
    {
        public NodeType Type;
        public string Value;
        public AtomNode(NodeType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
    public class SliceNode
    {
        public bool IsAtom;
        public AtomNode Start;
        public AtomNode End;
        public AtomNode Step;
        public SliceNode(bool isAtom, AtomNode start, AtomNode end, AtomNode step)
        {
            IsAtom = isAtom;
            Start = start;
            End = end;
            Step = step;
        }
    }
    public class AssignmentNode
    {
        public SliceNode Left;
        public SliceNode Right;
        public AssignmentNode(SliceNode left, SliceNode right)
        {
            Left = left;
            Right = right;
        }
    }
    public class AssignmentsNode
    {
        public List<AssignmentNode> Assignments;
        public AssignmentsNode(List<AssignmentNode> assignments)
        {
            Assignments = assignments;
        }
    }
    public class MappingNode
    {
        public NodeType Type;
        public List<AssignmentsNode> Dimensions;
        public int Upper;
        public MappingNode(NodeType type, List<AssignmentsNode> dimensions, int upper)
        {
            Type = type;
            Dimensions = dimensions;
            Upper = upper;
        }
    }
    public class AgentsNode
    {
        public bool IsAll;
        public List<string> Agents;
        public AgentsNode(bool isAll, List<string> agents)
        {
            IsAll = isAll;
            Agents = agents;
        }
    }
    public class Node
    {
        public bool IsRoot;
        public AgentsNode Agents;
        public List<MappingNode> Mappings;
        public Node(bool isRoot, AgentsNode agents, List<MappingNode> mappings)
        {
            IsRoot = isRoot;
            Agents = agents;
            Mappings = mappings;
        }
    }

    public class Parser
    {
        Lexer m_Lexer;
        Token m_CurrentToken;

        public Parser(Lexer lexer)
        {
            m_Lexer = lexer;
            m_CurrentToken = lexer.GetNextToken();
        }

        void Error()
        {
            throw new Exception("Invalid syntax at position " + m_Lexer.GetPosition() + " of the location string " + GetText());
        }

        void Eat(TokenType tokenType)
        {
            // Compare the current token type with the passed token type
            // and if they match then "eat" the current token
            // and assign the next token to the m_CurrentToken,
            // otherwise raise an exception.
            if (m_CurrentToken.Type == tokenType) m_CurrentToken = m_Lexer.GetNextToken();
            else Error();
        }

        Node Expr()
        {
            // <expr> ::= [ <agents> ] { <mapping> }
            bool isRoot = false;
            AgentsNode agents = null;
            List<MappingNode> mappings = new List<MappingNode>();
            if (m_CurrentToken.Type == TokenType.AT ||
                m_CurrentToken.Type == TokenType.NAME ||
                m_CurrentToken.Type == TokenType.STRING ||
                m_CurrentToken.Type == TokenType.INTEGER)
            {
                isRoot = true;
                agents = Agents();
            }
            while (m_CurrentToken.Type != TokenType.END)
            {
                mappings.Add(Mapping());
            }
            return new Node(isRoot, agents, mappings);
        }

        AgentsNode Agents()
        {
            // <agents> ::= "@" [ "@" { <atom> "@" } ]
            //            | <atom> "@" { <atom> "@" }
            bool isAll = false;
            List<string> agents = new List<string>();
            AtomNode atom;
            if (m_CurrentToken.Type == TokenType.AT)
            {
                Eat(TokenType.AT);
                if (m_CurrentToken.Type == TokenType.AT)
                {
                    Eat(TokenType.AT);
                    isAll = true;
                    while (m_CurrentToken.Type == TokenType.NAME ||
                           m_CurrentToken.Type == TokenType.STRING ||
                           m_CurrentToken.Type == TokenType.INTEGER)
                    {
                        atom = Atom();
                        Eat(TokenType.AT);
                        agents.Add(atom.Value);
                    }
                }
            }
            else if (m_CurrentToken.Type == TokenType.NAME ||
                     m_CurrentToken.Type == TokenType.STRING ||
                     m_CurrentToken.Type == TokenType.INTEGER)
            {
                atom = Atom();
                Eat(TokenType.AT);
                agents.Add(atom.Value);
                while (m_CurrentToken.Type == TokenType.NAME ||
                       m_CurrentToken.Type == TokenType.STRING ||
                       m_CurrentToken.Type == TokenType.INTEGER)
                {
                    atom = Atom();
                    Eat(TokenType.AT);
                    agents.Add(atom.Value);
                }
            }
            else Error();
            return new AgentsNode(isAll, agents);
        }

        MappingNode Mapping()
        {
            // <mapping> ::= { "." } "." <assignment>
            //             | { "." } "{" <assignments> "}"
            //             | { "." } "(" <assignments> { "," <assignments> } ")"
            //             | { "." } "[" <assignments> { "," <assignments> } "]"
            NodeType type = NodeType.UNSPECIFIED;
            List<AssignmentsNode> dimensions = new List<AssignmentsNode>();
            int upper = 0;
            while (m_CurrentToken.Type == TokenType.DOT)
            {
                upper++;
                Eat(TokenType.DOT);
            }
            switch (m_CurrentToken.Type)
            {
                case TokenType.LCURLY:
                    type = NodeType.DICT;
                    Eat(TokenType.LCURLY);
                    dimensions.Add(Assignments());
                    Eat(TokenType.RCURLY);
                    break;
                case TokenType.LPAREN:
                    type = NodeType.TUPLE;
                    Eat(TokenType.LPAREN);
                    dimensions.Add(Assignments());
                    while (m_CurrentToken.Type == TokenType.COMMA)
                    {
                        Eat(TokenType.COMMA);
                        dimensions.Add(Assignments());
                    }
                    Eat(TokenType.RPAREN);
                    break;
                case TokenType.LSQUARE:
                    type = NodeType.SEQUENCE;
                    Eat(TokenType.LSQUARE);
                    dimensions.Add(Assignments());
                    while (m_CurrentToken.Type == TokenType.COMMA)
                    {
                        Eat(TokenType.COMMA);
                        dimensions.Add(Assignments());
                    }
                    Eat(TokenType.RSQUARE);
                    break;
                default:
                    if (upper > 0)
                    {
                        // begin with dots
                        upper--;
                        type = NodeType.DICT;
                        List<AssignmentNode> assignments = new List<AssignmentNode>();
                        assignments.Add(Assignment());
                        dimensions.Add(new AssignmentsNode(assignments));
                    }
                    else
                    {
                        Error();
                        return null;
                    }
                    break;
            }
            return new MappingNode(type, dimensions, upper);
        }

        AssignmentsNode Assignments()
        {
            // <assignments> ::= <assignment> { "&" <assignment> }
            List<AssignmentNode> assignments = new List<AssignmentNode>();
            assignments.Add(Assignment());
            while (m_CurrentToken.Type == TokenType.AND)
            {
                Eat(TokenType.AND);
                assignments.Add(Assignment());
            }
            return new AssignmentsNode(assignments);
        }

        AssignmentNode Assignment()
        {
            // <assignment> ::= <slice> [ "=" <slice> ]
            SliceNode left = Slice();
            SliceNode right = null;
            if (m_CurrentToken.Type == TokenType.EQUAL)
            {
                Eat(TokenType.EQUAL);
                right = Slice();
            }
            return new AssignmentNode(left, right);
        }

        SliceNode Slice()
        {
            // <slice> ::= [ <atom> ] ":" [ <atom> ] [ ":" [ <atom> ] ]
            //           | <atom>
            bool isAtom = true;
            AtomNode start = null;
            AtomNode end = null;
            AtomNode step = null;
            if (m_CurrentToken.Type == TokenType.NAME ||
                m_CurrentToken.Type == TokenType.STRING ||
                m_CurrentToken.Type == TokenType.INTEGER)
            {
                start = Atom();
            }
            if (m_CurrentToken.Type == TokenType.COLON)
            {
                isAtom = false;
                Eat(TokenType.COLON);
                if (m_CurrentToken.Type == TokenType.NAME ||
                    m_CurrentToken.Type == TokenType.STRING ||
                    m_CurrentToken.Type == TokenType.INTEGER)
                {
                    end = Atom();
                }
                if (m_CurrentToken.Type == TokenType.COLON)
                {
                    Eat(TokenType.COLON);
                    if (m_CurrentToken.Type == TokenType.NAME ||
                        m_CurrentToken.Type == TokenType.STRING ||
                        m_CurrentToken.Type == TokenType.INTEGER)
                    {
                        step = Atom();
                    }
                }
            }
            else if (start == null) Error();
            return new SliceNode(isAtom, start, end, step);
        }

        AtomNode Atom()
        {
            // <atom> ::= ( NAME | STRING | INTEGER )
            Token token = m_CurrentToken;
            switch (token.Type)
            {
                case TokenType.NAME:
                    Eat(TokenType.NAME);
                    return new AtomNode(NodeType.NAME, token.Value);
                case TokenType.STRING:
                    Eat(TokenType.STRING);
                    return new AtomNode(NodeType.STRING, token.Value);
                case TokenType.INTEGER:
                    Eat(TokenType.INTEGER);
                    return new AtomNode(NodeType.INTEGER, token.Value);
                default:
                    Error();
                    return null;
            }
        }

        public Node Parse()
        {
            return Expr();
        }

        public string GetText()
        {
            return m_Lexer.GetText();
        }
    }
}