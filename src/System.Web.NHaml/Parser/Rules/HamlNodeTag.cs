using System.Web.NHaml.Crosscutting;
using System.Web.NHaml.Parser.Exceptions;

namespace System.Web.NHaml.Parser.Rules
{
    public enum WhitespaceRemoval
    {
        None = 0, Surrounding, Internal
    }

    public class HamlNodeTag : HamlNode
    {
        private string _tagName = string.Empty;
        private string _namespace = string.Empty;
        private WhitespaceRemoval _whitespaceRemoval = WhitespaceRemoval.None;

        public HamlNodeTag(IO.HamlLine nodeLine)
            : base(nodeLine)
        {
            IsSelfClosing = false;
            int pos = 0;

            SetNamespaceAndTagName(nodeLine.Content, ref pos);
            ParseClassAndIdNodes(nodeLine.Content, ref pos);
            ParseAttributes(nodeLine.Content, ref pos);
            ParseSpecialCharacters(nodeLine.Content, ref pos);
            HandleInlineContent(nodeLine.Content, ref pos);
        }

        protected override bool IsContentGeneratingTag
        {
            get { return true; }
        }

        private void SetNamespaceAndTagName(string content, ref int pos)
        {
            _tagName = GetTagName(content, ref pos);
            
            if (pos < content.Length
                && content[pos] == ':'
                && IsSelfClosing == false)
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
            if (pos >= content.Length) return;

            char attributeEndChar = HtmlStringHelper.GetAttributeTerminatingChar(content[pos]);
            if (attributeEndChar != '\0')
            {
                string attributes = HtmlStringHelper.ExtractTokenFromTagString(content, ref pos,
                                                                               new[] {attributeEndChar});
                if (attributes[attributes.Length - 1] != attributeEndChar)
                    throw new HamlMalformedTagException(
                        "Malformed HTML Attributes collection \"" + attributes + "\".", SourceFileLineNum);
                AddChild(new HamlNodeHtmlAttributeCollection(SourceFileLineNum, attributes));

                pos++;
            }
        }

        private void ParseSpecialCharacters(string content, ref int pos)
        {
            _whitespaceRemoval = WhitespaceRemoval.None;
            IsSelfClosing = false;

            while (pos < content.Length)
            {
                if (ParseWhitespaceRemoval(content, ref pos)) continue;
                if (ParseSelfClosing(content, ref pos)) continue;
                
                break;
            }
        }

        private bool ParseWhitespaceRemoval(string content, ref int pos)
        {
            if (_whitespaceRemoval != WhitespaceRemoval.None)
                return false;

            switch (content[pos])
            {
                case '>':
                    _whitespaceRemoval = WhitespaceRemoval.Surrounding;
                    pos++;
                    return true;
                case '<':
                    _whitespaceRemoval = WhitespaceRemoval.Internal;
                    pos++;
                    return true;
            }
            return false;
        }

        private bool ParseSelfClosing(string content, ref int pos)
        {
            if (IsSelfClosing || content[pos] != '/')
                return false;

            IsSelfClosing = true;
            pos++;
            return true;
        }

        private void HandleInlineContent(string content, ref int pos)
        {
            if (pos >= content.Length) return;

            string contentLine = content.Substring(pos).TrimStart();
            AddChild(new HamlNodeTextContainer(SourceFileLineNum, contentLine));
        }

        public string TagName
        {
            get { return _tagName; }
        }

        public bool IsSelfClosing { get; private set; }

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
