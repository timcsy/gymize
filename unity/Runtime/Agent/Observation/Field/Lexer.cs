using System;
using System.Collections;
using System.Collections.Generic;

namespace PAIA.Marenv
{
    public enum TokenType
    {
        AT,
        DOT,
        COMMA,
        AND,
        EQUAL,
        COLON,
        LPAREN,
        RPAREN,
        LSQUARE,
        RSQUARE,
        LCURLY,
        RCURLY,
        NAME,
        STRING,
        INTEGER,
        END
    }

    public class Token
    {
        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
        public TokenType Type { get; set; }
        public string Value { get; set; }
    }

    public class Lexer
    {
        string m_Text; // client string input, e.g. "@.camera[0]"
        int m_Position; // Position is an index into m_Text
        char m_CurrentChar;

        public Lexer(string text)
        {
            m_Text = text;
            m_Position = 0;
            if (m_Text.Length > 0) m_CurrentChar = m_Text[m_Position];
            else m_CurrentChar = '\0';
        }

        void Error()
        {
            throw new Exception("Invalid character at position " + m_Position + " of the field string " + m_Text);
        }

        void Advance()
        {
            // Advance the `m_Position` pointer and set the `m_CurrentChar` variable.
            m_Position++;
            if (m_Position > m_Text.Length - 1) m_CurrentChar = '\0'; // Indicates end of input
            else m_CurrentChar = m_Text[m_Position];
        }

        void SkipWhitespace()
        {
            while (m_CurrentChar != '\0' && Char.IsWhiteSpace(m_CurrentChar)) Advance();
        }

        string Name()
        {
            // Return a name consumed from the input.
            string result = "";
            while (m_CurrentChar != '\0' && Char.IsLetterOrDigit(m_CurrentChar) || m_CurrentChar == '_')
            {
                result += m_CurrentChar;
                Advance();
            }
            return result;
        }

        string Str()
        {
            // Return a string value consumed from the input.
            string result = "";
            char start = '\"';
            if (m_CurrentChar == '\'' || m_CurrentChar == '\"')
            {
                start = m_CurrentChar;
                Advance();
            }
            else Error();
            bool isEscaped = false;
            while (m_CurrentChar != '\0' && (m_CurrentChar != start || isEscaped))
            {
                if (!isEscaped && m_CurrentChar == '\\') isEscaped = true;
                else
                {
                    result += m_CurrentChar;
                    isEscaped = false;
                }
                Advance();
            }
            if (m_CurrentChar == start) Advance();
            else Error();
            return result;
        }

        string Integer()
        {
            // Return a (multidigit) integer consumed from the input.
            string result = "";
            if (m_CurrentChar == '-' || m_CurrentChar == '+')
            {
                result += m_CurrentChar;
                Advance();
            }
            while (m_CurrentChar != '\0' && Char.IsDigit(m_CurrentChar))
            {
                result += m_CurrentChar;
                Advance();
            }
            return result;
        }

        public Token GetNextToken()
        {
            // Lexical analyzer (also known as scanner or tokenizer)
            // This method is responsible for breaking a sentenceapart into tokens.
            // One token at a time.
            while (m_CurrentChar != '\0')
            {
                if (Char.IsWhiteSpace(m_CurrentChar))
                {
                    SkipWhitespace();
                    continue;
                }
                if (m_CurrentChar == '+' || m_CurrentChar == '-' || Char.IsDigit(m_CurrentChar))
                {
                    return new Token(TokenType.INTEGER, Integer());
                }
                if (m_CurrentChar == '_' || Char.IsLetter(m_CurrentChar))
                {
                    return new Token(TokenType.NAME, Name());
                }
                if (m_CurrentChar == '\'' || m_CurrentChar == '\"')
                {
                    return new Token(TokenType.STRING, Str());
                }
                switch (m_CurrentChar)
                {
                    case '@':
                        Advance();
                        return new Token(TokenType.AT, "@");
                    case '.':
                        Advance();
                        return new Token(TokenType.DOT, ".");
                    case ',':
                        Advance();
                        return new Token(TokenType.COMMA, ",");
                    case '&':
                        Advance();
                        return new Token(TokenType.AND, "&");
                    case '=':
                        Advance();
                        return new Token(TokenType.EQUAL, "=");
                    case ':':
                        Advance();
                        return new Token(TokenType.COLON, ":");
                    case '(':
                        Advance();
                        return new Token(TokenType.LPAREN, "(");
                    case ')':
                        Advance();
                        return new Token(TokenType.RPAREN, ")");
                    case '[':
                        Advance();
                        return new Token(TokenType.LSQUARE, "[");
                    case ']':
                        Advance();
                        return new Token(TokenType.RSQUARE, "]");
                    case '{':
                        Advance();
                        return new Token(TokenType.LCURLY, "{");
                    case '}':
                        Advance();
                        return new Token(TokenType.RCURLY, "}");
                    default:
                        Error();
                        return null;
                }
            }
            return new Token(TokenType.END, "");
        }

        public int GetPosition()
        {
            return m_Position;
        }

        public string GetText()
        {
            return m_Text;
        }
    }
}