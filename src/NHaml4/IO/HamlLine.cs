using NHaml4.Parser;
using System.Linq;

namespace NHaml4.IO
{
    public class HamlLine
    {
        public HamlLine(int sourceFileLineNum, string content, string indent, HamlRuleEnum hamlRule)
        {
            SourceFileLineNo = sourceFileLineNum;
            Content = content;
            Indent = indent;
            IndentCount = GetIndentCount(Indent);
            HamlRule = hamlRule;
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
        public string Content { get; private set; }
        public string Indent { get; private set; }
    }
}
