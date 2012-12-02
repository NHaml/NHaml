using NHaml.Parser;
using System.Linq;

namespace NHaml.IO
{
    public class HamlLine
    {
        public HamlLine(string content, HamlRuleEnum hamlRule,
            string indent = "", int sourceFileLineNum = -1, bool isInline = false)
        {
            SourceFileLineNo = sourceFileLineNum;
            Content = content;
            Indent = isInline ? "" : indent;
            IndentCount = GetIndentCount(indent);
            HamlRule = hamlRule;
            IsInline = isInline;
        }

        private int GetIndentCount(string indent)
        {
            if (string.IsNullOrEmpty(Content)) return 0;
            var chars = indent.ToArray();
            return chars.Sum(curChar => curChar == '\t' ? 2 : 1);
        }

        public int IndentCount { get; private set; }
        public int SourceFileLineNo { get; private set; }
        public HamlRuleEnum HamlRule { get; private set; }
        public string Indent { get; private set; }
        public string Content { get; set; }
        public bool IsInline { get; private set; }
    }
}
