using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser;

namespace NHaml4.IO
{
    public class HamlLine
    {
        enum ParseState { WhiteSpace, Content };
        private int _indentCount;
        private HamlRuleEnum _hamlRule;
        private string _content;

        public HamlLine(string currentLine)
        {
            ParseHamlLine(currentLine);
        }

        public int IndentCount
        {
            get { return _indentCount; }
        }

        public HamlRuleEnum HamlRule
        {
            get { return _hamlRule; }
        }

        public string Content
        {
            get { return _content; }
        }

        private void ParseHamlLine(string currentLine)
        {
            _indentCount = 0;
            int curIndex = 0;

            while (curIndex < currentLine.Length)
            {
                if (currentLine[curIndex] == ' ')
                    _indentCount++;
                else if (currentLine[curIndex] == '\t')
                    _indentCount += 2;
                else
                    break;
                curIndex++;
            }

            if (curIndex == currentLine.Length)
            {
                _indentCount = 0;
                _hamlRule = HamlRuleEnum.PlainText;
                _content = "";
                return;
            }

            _content = currentLine.Substring(curIndex);

            _hamlRule = ParseHamlRule();
        }

        private HamlRuleEnum ParseHamlRule()
        {
            if (_content == "") return HamlRuleEnum.PlainText;

            if (_content.StartsWith("!!!"))
            {
                _content = (_content.Length > 3 ?_content.Substring(3) : "");
                return HamlRuleEnum.DocType;
            }
            if (_content.StartsWith("-#"))
            {
                _content = (_content.Length > 2 ? _content.Substring(2) : "");
                return HamlRuleEnum.HamlComment;
            }
            if (_content.StartsWith("%"))
            {
                _content = (_content.Length > 1 ? _content.Substring(1) : "");
                return HamlRuleEnum.Tag;
            }
            if (_content.StartsWith("#"))
            {
                _content = (_content.Length > 1 ? _content.Substring(1) : "");
                return HamlRuleEnum.DivId;
            }
            if (_content.StartsWith("."))
            {
                _content = (_content.Length > 1 ? _content.Substring(1) : "");
                return HamlRuleEnum.DivClass;
            }
            if (_content.StartsWith("/"))
            {
                _content = (_content.Length > 1 ? _content.Substring(1) : "");
                return HamlRuleEnum.HtmlComment;
            }
            if (_content.StartsWith("="))
            {
                _content = (_content.Length > 1 ? _content.Substring(1) : "");
                return HamlRuleEnum.Evaluation;
            }
            return HamlRuleEnum.PlainText;
        }
    }
}
