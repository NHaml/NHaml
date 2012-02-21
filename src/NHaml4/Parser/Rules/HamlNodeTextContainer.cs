using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser.Exceptions;
using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTextContainer : HamlNode
    {
        public HamlNodeTextContainer(IO.HamlLine nodeLine)
            : base(nodeLine)
        {
            ParseFragments(nodeLine);
        }

        private void ParseFragments(IO.HamlLine nodeLine)
        {
            int index = 0;
            while (index < nodeLine.Content.Length)
            {
                var node = GetNextNode(nodeLine.Content, ref index);
                AddChild(node);
            }
        }

        private HamlNode GetNextNode(string content, ref int index)
        {
            int startIndex = index;
            bool isInTag = IsTagToken(content, index);
            bool isEscaped = false;

            string result = string.Empty;

            for (; index < content.Length; index++)
            {
                if (isInTag)
                {
                    result += content[index];
                    if (IsEndTagToken(content, index))
                    {
                        index++;
                        if (result.Length > 3)
                            return new HamlNodeTextVariable(result, SourceFileLineNum);
                        else
                            return new HamlNodeTextLiteral(result, SourceFileLineNum);
                    }
                }
                else if (IsTagToken(content, index))
                {
                    if (isEscaped == false)
                        return new HamlNodeTextLiteral(result, SourceFileLineNum);
                    else
                    {
                        result = RemoveEscapeCharacter(result) + content[index];
                    }
                }
                else
                {
                    result += content[index];
                    if (IsEscapeToken(content, index))
                    {
                        if (isEscaped) result = RemoveEscapeCharacter(result);
                        isEscaped = !isEscaped;
                    }
                }
            }

            if (isInTag)
                throw new HamlMalformedVariableException(result, SourceFileLineNum);

            return new HamlNodeTextLiteral(result, SourceFileLineNum);
        }

        private static string RemoveEscapeCharacter(string result)
        {
            result = result.Substring(0, result.Length - 1);
            return result;
        }

        private bool IsEscapeToken(string content, int index)
        {
            return (content[index] == '\\');
        }

        private bool IsEndTagToken(string content, int index)
        {
            return (content[index] == '}');
        }

        private bool IsTagToken(string content, int index)
        {
            return (index < content.Length - 1
                    && content[index] == '#'
                    && content[index + 1] == '{'); 
        }

        public bool IsWhitespace()
        {
            return Content.Trim().Length == 0;
        }
    }
}
