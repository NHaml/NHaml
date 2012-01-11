using System;
using System.IO;
using System.Text;
using NHaml4.Parser.Exceptions;

namespace NHaml4.IO
{
    public class HamlFileLexer
    {
        bool _eof;

        public HamlFile Read(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var result = new HamlFile();

            while (_eof == false)
            {
                string currentLine = ReadLine(reader);
                while (IsPartialTag(currentLine))
                {
                    if (_eof)
                        throw new HamlMalformedTagException("Multi-line tag found with no end token.");
                    currentLine += " " + ReadLine(reader);
                }

                result.AddLine(new HamlLine(currentLine));
            }

            return result;
        }

        private bool IsPartialTag(string currentLine)
        {
            bool inHtmlAttributes = false;
            bool inSingleQuote = false;
            bool inDoubleQuote = false;

            foreach (char curChar in currentLine)
            {
                if (inSingleQuote)
                {
                    if (curChar == '\'') inSingleQuote = false;
                }
                else if (inDoubleQuote)
                {
                    if (curChar == '\"') inDoubleQuote = false;
                }
                else if (inHtmlAttributes)
                {
                    if (curChar == '\'')
                        inSingleQuote = true;
                    else if (curChar == '\"')
                        inDoubleQuote = true;
                    else if (curChar == ')')
                    {
                        inHtmlAttributes = false;
                        break;
                    }
                }
                else
                {
                    if (curChar == '(')
                        inHtmlAttributes = true;
                    else if (Char.IsWhiteSpace(curChar))
                        break;
                }
            }
            return inHtmlAttributes;
        }

        private string ReadLine(TextReader reader)
        {
            var line = new StringBuilder();
            int r = reader.Read();
            while (r >= 0)
            {
                char c = Convert.ToChar(r);
                if (c == '\n') break;
                if (c == '\r')
                {
                    HandleCrLfSequence(reader);
                    break;
                }

                line.Append(c);
                r = reader.Read();
            }
            if (r <= 0)
                _eof = true;
            return line.ToString();
        }

        private static void HandleCrLfSequence(TextReader reader)
        {
            int peek = reader.Peek();
            if ((peek != -1) && (Convert.ToChar(peek) == '\n'))
                reader.Read();
        }
    }
}