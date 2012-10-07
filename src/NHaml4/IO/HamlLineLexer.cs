using System.Collections.Generic;
using NHaml4.Parser;
using System;

namespace NHaml4.IO
{
    public class HamlLineLexer
    {
        public IEnumerable<HamlLine> ParseHamlLine(string currentLine, int currentLineIndex)
        {
            int whiteSpaceIndex = 0;

            while (whiteSpaceIndex < currentLine.Length
                && (currentLine[whiteSpaceIndex] == ' ' || currentLine[whiteSpaceIndex] == '\t'))
            {
                whiteSpaceIndex++;
            }

            string indent = currentLine.Substring(0, whiteSpaceIndex);
            string content = (whiteSpaceIndex == currentLine.Length) ? "" : currentLine.Substring(whiteSpaceIndex);
            content = AddImplicitDivTag(content);
            var hamlRule = HamlRuleFactory.ParseHamlRule(ref content);

            return new List<HamlLine>
                       {
                           new HamlLine(currentLineIndex, content, indent, hamlRule)
                       };
        }

        private string AddImplicitDivTag(string content)
        {
            if (content.Length == 0) return "";
            if (content[0] == '.' || content[0] == '#')
                content = "%" + content;
            return content;
        }

        public bool IsPartialTag(string currentLine)
        {
            bool inAttributes = false;
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
                else if (inAttributes)
                {
                    if (curChar == '\'')
                        inSingleQuote = true;
                    else if (curChar == '\"')
                        inDoubleQuote = true;
                    else if (curChar == ')' || curChar == '}')
                    {
                        inAttributes = false;
                        break;
                    }
                }
                else
                {
                    if (curChar == '(' || curChar == '{')
                        inAttributes = true;
                    else if (Char.IsWhiteSpace(curChar))
                        break;
                }
            }
            return inAttributes;
        }
    }
}
