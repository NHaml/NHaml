using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser.Exceptions;

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
                string token = GetNextToken(nodeLine.Content, ref index);
                if (token.Length > 3 && token.StartsWith("#{") && token.EndsWith("}"))
                    AddChild(new HamlNodeTextVariable(nodeLine));
                else
                    AddChild(new HamlNodeTextLiteral(nodeLine));
            }
        }

        private string GetNextToken(string content, ref int index)
        {
            int startIndex = index;

            bool isInTag = IsTagToken(content, index);
            for (; index < content.Length; index++)
            {
                if (isInTag)
                {
                    if (IsEndTagToken(content, index))
                    {
                        index++;
                        isInTag = false;
                        break;
                    }
                }
                else
                {
                    if (IsTagToken(content, index)) break;
                }
            }
            return content.Substring(startIndex, index - startIndex);
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
