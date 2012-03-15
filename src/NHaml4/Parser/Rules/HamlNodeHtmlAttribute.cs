using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Crosscutting;
using NHaml4.Parser.Exceptions;
using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeHtmlAttribute : HamlNode
    {
        private string _name = string.Empty;
        private char _quoteChar = '\'';

        public HamlNodeHtmlAttribute(int sourceFileLineNo, string nameValuePair)
            : base(sourceFileLineNo, nameValuePair)
        {
            int index = 0;
            _name = HtmlStringHelper.ExtractTokenFromTagString(Content, ref index, new[] { '=', '\0' });
            if (_name.EndsWith("=")) _name = _name.Substring(0, _name.Length - 1);

            if (!string.IsNullOrEmpty(_name))
            {
                if (index < Content.Length)
                {
                    string value = Content.Substring(index + 1);
                    if (IsQuoted(value))
                    {
                        value = RemoveQuotes(value);
                    }
                    else
                    {
                        value = "#{" + value + "}";
                    }
                    var valueNode = new HamlNodeTextContainer(SourceFileLineNum, value);
                    AddChild(valueNode);
                }
            }
            else
            {
                throw new HamlMalformedTagException("Malformed HTML attribute \"" + nameValuePair + "\"", SourceFileLineNum);
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public char QuoteChar
        {
            get
            {
                return _quoteChar;
            }
        }

        private bool IsQuoted(string input)
        {
          return ((input[0] == '\'' && input[input.Length - 1] == '\'')
                || (input[0] == '"' && input[input.Length - 1] == '"'));
        }

        private string RemoveQuotes(string input)
        {
            if (input.Length < 2) return input;

            if ((input[0] == '\'' && input[input.Length - 1] == '\'')
                || (input[0] == '"' && input[input.Length - 1] == '"'))
            {
                _quoteChar = input[0];
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }
    }
}
