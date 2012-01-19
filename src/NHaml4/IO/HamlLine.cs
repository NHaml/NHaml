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
        private int _sourceFileLineNo;

        public HamlLine(string currentLine, int sourceFileLineNo)
        {
            ParseHamlLine(currentLine);
            _sourceFileLineNo = sourceFileLineNo;
        }

        public int IndentCount
        {
            get { return _indentCount; }
        }

        public int SourceFileLineNo
        {
            get { return _sourceFileLineNo; }
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

            if (string.IsNullOrEmpty(_content)) _indentCount = 0;

            AddImplicitDivTag();

            _hamlRule = ParseHamlRule();
        }

        private void AddImplicitDivTag()
        {
            if (_content.Length == 0) return;
            if (_content[0] == '.' || _content[0] == '#')
                _content = "%" + _content;
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
