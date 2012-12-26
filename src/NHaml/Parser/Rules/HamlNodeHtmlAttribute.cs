using System.Web.NHaml.Crosscutting;
using System.Web.NHaml.Parser.Exceptions;

namespace System.Web.NHaml.Parser.Rules
{
    public class HamlNodeHtmlAttribute : HamlNode
    {
        private string _name = string.Empty;
        private char _quoteChar = '\'';

        public HamlNodeHtmlAttribute(int sourceFileLineNo, string nameValuePair)
            : base(sourceFileLineNo, nameValuePair)
        {
            int index = 0;
            ParseName(ref index);
            ParseValue(index);
        }

        private void ParseValue(int index)
        {
            if (index >= Content.Length) return;

            string value = Content.Substring(index + 1);
            value = IsQuoted(value)
                ? RemoveQuotes(value)
                : "#{" + value + "}";

            AddChild(new HamlNodeTextContainer(SourceFileLineNum, value));
        }

        private void ParseName(ref int index)
        {
            string result = HtmlStringHelper.ExtractTokenFromTagString(Content, ref index, new[] { '=', '\0' });
            if (string.IsNullOrEmpty(result))
                throw new HamlMalformedTagException("Malformed HTML attribute \"" + Content + "\"", SourceFileLineNum);

            _name = result.TrimEnd('=');
        }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }

        public string Name
        {
            get { return _name; }
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
