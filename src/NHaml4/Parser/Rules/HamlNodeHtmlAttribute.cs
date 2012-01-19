using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Crosscutting;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeHtmlAttribute : HamlNode
    {
        private string _name = string.Empty;
        private string _value = string.Empty;

        public HamlNodeHtmlAttribute(int sourceFileLineNo, string nameValuePair)
            : base(sourceFileLineNo, nameValuePair)
        {
            int index = 0;
            _name = HtmlStringHelper.ExtractTokenFromTagString(Content, ref index, new[] { '=', '\0' });
            if (_name.EndsWith("=")) _name = _name.Substring(0, _name.Length - 1);

            if (!string.IsNullOrEmpty(_name))
            {
                if (index < Content.Length)
                    _value = Content.Substring(index + 1);
            }
            else
            {
                throw new HamlMalformedTagException("Malformed HTML attribute \"" + nameValuePair + "\"", SourceFileLineNo);
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public string ValueWithoutQuotes
        {
            get
            {
                if (_value.Length < 2) return _value;

                if ((_value[0] == '\'' && _value[_value.Length - 1] == '\'')
                    || (_value[0] == '"' && _value[_value.Length - 1] == '"'))
                    return _value.Substring(1, _value.Length - 2);
                
                return _value;
            }
        }
    }
}
