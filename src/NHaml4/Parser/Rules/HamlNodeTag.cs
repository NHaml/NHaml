using System;
using System.Linq;
using System.Collections.Generic;
using NHaml4.Crosscutting;
using NHaml4.IO;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Parser.Rules
{
    public enum WhitespaceRemoval
    {
        None = 0, Surrounding, Internal
    }

    public class HamlNodeTag : HamlNode
    {
        private string _tagName = string.Empty;
        private string _namespace = string.Empty;
        private bool _isSelfClosing = false;
        private WhitespaceRemoval _whitespaceRemoval = WhitespaceRemoval.None;

        public HamlNodeTag(IO.HamlLine nodeLine)
            : base(nodeLine)
        {
            int pos = 0;

            SetNamespaceAndTagName(nodeLine.Content, ref pos);
            ParseClassAndIdNodes(nodeLine.Content, ref pos);
            ParseAttributes(nodeLine.Content, ref pos);
            ParseSpecialCharacters(nodeLine.Content, ref pos);
            HandleInlineContent(nodeLine.Content, ref pos);
        }

        public override bool IsContentGeneratingTag
        {
            get { return true; }
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
                char attributeEndChar = '\0';

                if (content[pos] == '(')
                    attributeEndChar = ')';
                else if (content[pos] == '{')
                    attributeEndChar = '}';

                if (attributeEndChar != '\0')
                {
                    string attributes = HtmlStringHelper.ExtractTokenFromTagString(content, ref pos, new[] { attributeEndChar });
                    if (attributes[attributes.Length - 1] != attributeEndChar)
                        throw new HamlMalformedTagException("Malformed HTML Attributes collection \"" + attributes + "\".", SourceFileLineNum);
                    pos++;
                    var attributesNode = new HamlNodeHtmlAttributeCollection(SourceFileLineNum, attributes);
                    AddChild(attributesNode);
                }
            }
        }

        private void ParseSpecialCharacters(string content, ref int pos)
        {
            _whitespaceRemoval = WhitespaceRemoval.None;
            _isSelfClosing = false;

            while (pos < content.Length)
            {
                if (ParseWhitespaceRemoval(content, ref pos) == false
                    && ParseSelfClosing(content, pos) == false)
                    break;
            }
        }

        private bool ParseSelfClosing(string content, int pos)
        {
            if (_isSelfClosing || content[pos] != '/')
                return false;

            _isSelfClosing = true;
            pos++;
            return true;
        }

        private bool ParseWhitespaceRemoval(string content, ref int pos)
        {
            if (_whitespaceRemoval != WhitespaceRemoval.None)
                return false;

            if (content[pos] == '>')
            {
                _whitespaceRemoval = WhitespaceRemoval.Surrounding;
                pos++;
                return true;
            }
            else if (content[pos] == '<')
            {
                _whitespaceRemoval = WhitespaceRemoval.Internal;
                pos++;
                return true;
            }
            return false;
        }

        private void HandleInlineContent(string content, ref int pos)
        {
            if (pos < content.Length)
            {
                string contentLine = content.Substring(pos).TrimStart();
                AddChild(new HamlNodeTextContainer(SourceFileLineNum, contentLine));
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

        public WhitespaceRemoval WhitespaceRemoval
        {
            get { return _whitespaceRemoval; }
        }

        public string Namespace
        {
            get { return _namespace; }
        }

        private string GetTagName(string content, ref int pos)
        {
            string result = GetHtmlToken(content, ref pos);

            return string.IsNullOrEmpty(result) ? "div" : result;
        }

        private void ParseTagIdNode(string content, ref int pos)
        {
            pos++;
            string tagId = GetHtmlToken(content, ref pos);
            var newTag = new HamlNodeTagId(SourceFileLineNum, tagId);
            AddChild(newTag);
        }

        private void ParseClassNode(string content, ref int pos)
        {
            pos++;
            string className = GetHtmlToken(content, ref pos);
            var newTag = new HamlNodeTagClass(SourceFileLineNum, className);
            AddChild(newTag);
        }

        private string GetHtmlToken(string content, ref int pos)
        {
            int startIndex = pos;
            while (pos < content.Length)
            {
                if (HtmlStringHelper.IsHtmlIdentifierChar(content[pos]))
                    pos++;
                else
                    break;
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
