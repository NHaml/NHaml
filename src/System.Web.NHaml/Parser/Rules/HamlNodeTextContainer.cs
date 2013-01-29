using System.Web.NHaml.IO;
using System.Web.NHaml.Parser.Exceptions;

namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeTextContainer : HamlNode
    {
        public HamlNodeTextContainer(HamlLine nodeLine)
            : base(nodeLine)
        {
            ParseFragments(nodeLine.Content);
        }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }

        public HamlNodeTextContainer(int sourceFileLineNo, string content)
            : base(sourceFileLineNo, content)
        {
            if (string.IsNullOrEmpty(content))
                AddChild(new HamlNodeTextLiteral(sourceFileLineNo, content));
            else
                ParseFragments(content);
        }

        private void ParseFragments(string content)
        {
            int index = 0;
            while (index < content.Length)
            {
                var node = GetNextNode(content, ref index);
                AddChild(node);
            }
        }

        private HamlNode GetNextNode(string content, ref int index)
        {
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
                        return (result.Length > 3)
                            ? new HamlNodeTextVariable(SourceFileLineNum, result)
                            : (HamlNode)new HamlNodeTextLiteral(SourceFileLineNum, result);
                    }
                }
                else if (IsTagToken(content, index))
                {
                    if (isEscaped == false)
                        return new HamlNodeTextLiteral(SourceFileLineNum, result);
                    result = RemoveEscapeCharacter(result) + content[index];
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

            return new HamlNodeTextLiteral(SourceFileLineNum, result);
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
