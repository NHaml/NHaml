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
        protected HamlRuleEnum _hamlRule;
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

            _content = (whiteSpaceIndex == currentLine.Length) ? "" : currentLine.Substring(whiteSpaceIndex);

            if (string.IsNullOrEmpty(_content)) _indentCount = 0;

            _hamlRule = ParseHamlRule();
        }

        private HamlRuleEnum ParseHamlRule()
        {
            if (_content == "") return HamlRuleEnum.PlainText;

            if (_content.StartsWith("!!!"))
            {
                _content = (_content.Length > 3 ? _content.Substring(3) : "");
                return HamlRuleEnum.DocType;
            }
            else if (_content.StartsWith("-#"))
            {
                _content = (_content.Length > 2 ? _content.Substring(2) : "");
                return HamlRuleEnum.HamlComment;
            }
            else if (_content.StartsWith("%"))
            {
                _content = (_content.Length > 1 ? _content.Substring(1) : "");
                return HamlRuleEnum.Tag;
            }
            //if (_content.StartsWith("#"))
            //{
            //    _content = (_content.Length > 1 ? _content.Substring(1) : "");
            //    return HamlRuleEnum.DivId;
            //}
            //if (_content.StartsWith("."))
            //{
            //    _content = (_content.Length > 1 ? _content.Substring(1) : "");
            //    return HamlRuleEnum.DivClass;
            //}
            else if (_content.StartsWith("/"))
            {
                _content = (_content.Length > 1 ? _content.Substring(1) : "");
                return HamlRuleEnum.HtmlComment;
            }
            //if (_content.StartsWith("="))
            //{
            //    _content = (_content.Length > 1 ? _content.Substring(1) : "");
            //    return HamlRuleEnum.Evaluation;
            //}
            return HamlRuleEnum.PlainText;
        }
    }
}
