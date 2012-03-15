using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser;

namespace NHaml4.IO
{
    public class HamlLine
    {
        private int _indentCount;
        private HamlRuleEnum _hamlRule;
        private string _content;
        private string _indent;
        private readonly int _sourceFileLineNum;

        public HamlLine(string currentLine, int sourceFileLineNum)
        {
            ParseHamlLine(currentLine);
            _sourceFileLineNum = sourceFileLineNum;
            _hamlRule = HamlRuleFactory.ParseHamlRule(ref _content);
            AddImplicitDivTag();

            if (string.IsNullOrEmpty(_content)) _indentCount = 0;
        }

        public int IndentCount
        {
            get { return _indentCount; }
        }

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

        public string Indent
        {
            get { return _indent; }
        }

        private void ParseHamlLine(string currentLine)
        {
            _indentCount = 0;
            int whiteSpaceIndex = 0;
            while (whiteSpaceIndex < currentLine.Length)
            {
                if (currentLine[whiteSpaceIndex] == ' ')
                    _indentCount++;
                else if (currentLine[whiteSpaceIndex] == '\t')
                    _indentCount += 2;
                else
                    break;
                whiteSpaceIndex++;
            }

            _indent = currentLine.Substring(0, whiteSpaceIndex);
            _content = (whiteSpaceIndex == currentLine.Length) ? "" : currentLine.Substring(whiteSpaceIndex);
        }

        private void AddImplicitDivTag()
        {
            if (_hamlRule == HamlRuleEnum.DivClass || _hamlRule == HamlRuleEnum.DivId)
                _hamlRule = HamlRuleEnum.Tag;
        }
    }
}
