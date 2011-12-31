using System;
using System.Collections.Generic;
using NHaml4.Crosscutting;
using NHaml4.IO;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTag : HamlNode
    {
        private string _tagName = string.Empty;
        private string _namespace = string.Empty;
        private string _tagClass = string.Empty;
        private string _tagId = string.Empty;
        private readonly IList<KeyValuePair<string, string>> _attributes = new List<KeyValuePair<string, string>>();
        private bool _isSelfClosing = false;

        public HamlNodeTag(IO.HamlLine nodeLine)
            : base(nodeLine)
        {
            int pos = 0;

            SetNamespaceAndTagName(nodeLine.Content, ref pos);
            SetClassAndId(nodeLine.Content, ref pos);
            HandleInlineContent(nodeLine.Content, ref pos);
        }

        private void SetNamespaceAndTagName(string content, ref int pos)
        {
            _tagName = GetTagName(content, ref pos);
            
            if (pos < content.Length
                && content[pos] == ':'
                && _isSelfClosing == false)
            {
                pos++;
                _namespace = _tagName;
                _tagName = GetTagName(content, ref pos);
            }
        }


        private void SetClassAndId(string content, ref int pos)
        {
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

        private void HandleInlineContent(string content, ref int pos)
        {
            if (pos >= content.Length) return;

            var contentLine = new HamlLine(content.Substring(pos).TrimStart());
            Add(new HamlNodeText(contentLine));
        }

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

        public bool IsSelfClosing
        {
            get { return _isSelfClosing; }
        }

        public string Namespace
        {
            get { return _namespace; }
        }

        private string GetTagName(string content, ref int pos)
        {
            string result = GetHtmlToken(content, ref pos);
            if (pos < content.Length && content[pos] == '/')
            {
                _isSelfClosing = true;
                pos++;
            }
            else
            {
                _isSelfClosing = false;
            }

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

        public string NamespaceQualifiedTagName {
            get
            {
                return string.IsNullOrEmpty(_namespace)
                    ? _tagName
                    : _namespace + ":" + _tagName;
            }
        }
    }
}
