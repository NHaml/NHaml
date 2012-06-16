using NHaml4.Parser;

namespace NHaml4.IO
{
    public class HamlLine
    {
        private HamlRuleEnum _hamlRule;
        private string _content;
        private readonly int _sourceFileLineNum;

        public HamlLine(string currentLine, int sourceFileLineNum)
        {
            ParseHamlLine(currentLine);
            _sourceFileLineNum = sourceFileLineNum;
            _hamlRule = HamlRuleFactory.ParseHamlRule(ref _content);
            AddImplicitDivTag();

            if (string.IsNullOrEmpty(currentLine.Trim())) IndentCount = 0;
        }

        public int IndentCount { get; private set; }

        public int SourceFileLineNo
        {
            get { return _sourceFileLineNum; }
        }

        public HamlRuleEnum HamlRule
        {
            get { return _hamlRule; }
            protected set { _hamlRule = value; }
        }

        public string Content
        {
            get { return _content; }
        }

        public string Indent { get; private set; }

        private void ParseHamlLine(string currentLine)
        {
            IndentCount = 0;
            int whiteSpaceIndex = 0;
            while (whiteSpaceIndex < currentLine.Length)
            {
                if (currentLine[whiteSpaceIndex] == ' ')
                    IndentCount++;
                else if (currentLine[whiteSpaceIndex] == '\t')
                    IndentCount += 2;
                else
                    break;
                whiteSpaceIndex++;
            }

            Indent = currentLine.Substring(0, whiteSpaceIndex);
            _content = (whiteSpaceIndex == currentLine.Length) ? "" : currentLine.Substring(whiteSpaceIndex);
        }

        private void AddImplicitDivTag()
        {
            if (_hamlRule == HamlRuleEnum.DivClass || _hamlRule == HamlRuleEnum.DivId)
                _hamlRule = HamlRuleEnum.Tag;
        }
    }
}
