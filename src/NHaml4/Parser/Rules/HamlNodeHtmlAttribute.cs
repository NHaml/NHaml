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
            _name = ParseName(ref index);
            if (index < Content.Length)
            {
                var value = ParseValue(index);
                AddChild(new HamlNodeTextContainer(SourceFileLineNum, value));
            }
        }

        private string ParseValue(int index)
        {
            string value = Content.Substring(index + 1);
            value = IsQuoted(value)
                ? RemoveQuotes(value)
                : value = "#{" + value + "}";
            return value;
        }

        private string ParseName(ref int index)
        {
            string result = HtmlStringHelper.ExtractTokenFromTagString(Content, ref index, new[] { '=', '\0' });
            if (string.IsNullOrEmpty(result))
                throw new HamlMalformedTagException("Malformed HTML attribute \"" + Content + "\"", SourceFileLineNum);

            return result.TrimEnd('=');
        }

        public override bool IsContentGeneratingTag
        {
            get { return true; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public char QuoteChar
        {
            get { return _quoteChar; }
        }

        private bool IsQuoted(string input)
        {
          return ((input[0] == '\'' && input[input.Length - 1] == '\'')
                || (input[0] == '"' && input[input.Length - 1] == '"'));
        }

        private string RemoveQuotes(string input)
        {
            if (input.Length < 2 || IsQuoted(input) == false)
                return input;

            _quoteChar = input[0];
            return input.Substring(1, input.Length - 2);
        }
    }
}
