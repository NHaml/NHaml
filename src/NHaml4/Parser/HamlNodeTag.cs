using System;
using System.Collections.Generic;
using NHaml4.Crosscutting;

namespace NHaml4.Parser
{
    public class HamlNodeTag : HamlNode
    {
        private readonly string _tagName = string.Empty;
        private readonly IList<KeyValuePair<string, string>> _attributes = new List<KeyValuePair<string, string>>();
        private string _tagClass;
        private string _tagId;

        public HamlNodeTag(IO.HamlLine nodeLine)
            : this(nodeLine.Content)
        { }

        public IList<KeyValuePair<string, string>> Attributes
        {
            get
            {
                var result = new List<KeyValuePair<string, string>>();
                if (!string.IsNullOrEmpty(_tagClass)) result.Add(new KeyValuePair<string, string>("class", _tagClass));
                if (!string.IsNullOrEmpty(_tagId)) result.Add(new KeyValuePair<string, string>("id", _tagId));
                                 
                result.AddRange(_attributes);
                return result;
            }
        }

        public string TagName
        {
            get { return _tagName; }
        }

        public HamlNodeTag(string content)
        {
            int pos = 0;
            _tagName = GetTagName(content, ref pos);

            while (pos < content.Length)
            {
                if (content[pos] == '#')
                    SetTagId(content, ref pos);
                else if (content[pos] == '.')
                    SetClassName(content, ref pos);
                else
                    break;
            }
        }

        private string GetTagName(string content, ref int pos)
        {
            string result = GetHtmlToken(content, ref pos);
            return string.IsNullOrEmpty(result) ? "div" : result;
        }

        private void SetTagId(string content, ref int pos)
        {
            pos++;
            _tagId = GetHtmlToken(content, ref pos);
        }

        private void SetClassName(string content, ref int pos)
        {
            pos++;
            string className = GetHtmlToken(content, ref pos);
            if (!string.IsNullOrEmpty(className))
            {
                _tagClass = string.IsNullOrEmpty(_tagClass)
                                ? className
                                : _tagClass + " " + className;
            }
        }

        private string GetHtmlToken(string content, ref int pos)
        {
            int startIndex = pos;
            while (pos < content.Length)
            {
                if (HtmlCharHelper.IsHtmlIdentifierChar(content[pos]))
                {
                    pos++;
                }
                else
                {
                    break;
                }
            }
            return content.Substring(startIndex, pos - startIndex);
        }
    }
}
