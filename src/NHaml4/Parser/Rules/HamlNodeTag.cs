using System;
using System.Linq;
using System.Collections.Generic;
using NHaml4.Crosscutting;
using NHaml4.IO;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Parser.Rules
{
    public class HamlNodeTag : HamlNode
    {
        private string _tagName = string.Empty;
        private string _namespace = string.Empty;
        private bool _isSelfClosing = false;

        public HamlNodeTag(IO.HamlLine nodeLine)
            : base(nodeLine)
        {
            int pos = 0;

            SetNamespaceAndTagName(nodeLine.Content, ref pos);
            ParseClassAndIdNodes(nodeLine.Content, ref pos);
            ParseAttributes(nodeLine.Content, ref pos);
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

        private void ParseClassAndIdNodes(string content, ref int pos)
        {
            while (pos < content.Length)
            {
                if (content[pos] == '#')
                    ParseTagIdNode(content, ref pos);
                else if (content[pos] == '.')
                    ParseClassNode(content, ref pos);
                else
                    break;
            }
        }

        private void ParseAttributes(string content, ref int pos)
        {
            if (pos < content.Length)
            {
                if (content[pos] == '(')
                {
                    string attributes = HtmlStringHelper.ExtractTokenFromTagString(content, ref pos, new[] { ')' });
                    if (attributes[attributes.Length - 1] != ')')
                        throw new HamlMalformedTagException("Malformed HTML Attributes collection \"" + attributes + "\".");
                    pos++;
                    var attributesNode = new HamlNodeHtmlAttributeCollection(attributes);
                    Add(attributesNode);
                }
            }
        }
       
        private void HandleInlineContent(string content, ref int pos)
        {
            if (pos < content.Length)
            {
                var contentLine = new HamlLine(content.Substring(pos).TrimStart());
                Add(new HamlNodeText(contentLine));
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

        private void ParseTagIdNode(string content, ref int pos)
        {
            pos++;
            string tagId = GetHtmlToken(content, ref pos);
            var newTag = new HamlNodeTagId(tagId);
            Add(newTag);
        }

        private void ParseClassNode(string content, ref int pos)
        {
            pos++;
            string className = GetHtmlToken(content, ref pos);
            var newTag = new HamlNodeTagClass(className);
            Add(newTag);
        }

        private string GetHtmlToken(string content, ref int pos)
        {
            int startIndex = pos;
            while (pos < content.Length)
            {
                if (HtmlStringHelper.IsHtmlIdentifierChar(content[pos]))
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
