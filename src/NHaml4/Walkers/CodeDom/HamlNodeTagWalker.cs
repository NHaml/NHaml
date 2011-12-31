using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTagWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeTagWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeTag = node as HamlNodeTag;
            if (nodeTag == null)
                throw new InvalidCastException("HamlNodeTagWalker requires that HamlNode object be of type HamlNodeTag.");

            string attributesMarkup = GetAttributes(nodeTag.Attributes);
            
            _classBuilder.Append(nodeTag.Indent);
            if (nodeTag.IsSelfClosing || _options.IsAutoClosingTag(nodeTag.TagName))
                RenderSelfClosingTag(nodeTag, attributesMarkup);
            else
                RenderStandardTag(nodeTag, attributesMarkup);
        }

        private void RenderSelfClosingTag(HamlNodeTag nodeTag, string attributesMarkup)
        {
            string tagFormat = (_options.HtmlVersion == HtmlVersion.XHtml ? "<{0}{1} />" : "<{0}{1}>");
            _classBuilder.AppendFormat(tagFormat, nodeTag.NamespaceQualifiedTagName, attributesMarkup);

        }

        private void RenderStandardTag(HamlNodeTag nodeTag, string attributesMarkup)
        {
            _classBuilder.AppendFormat("<{0}{1}>", nodeTag.NamespaceQualifiedTagName, attributesMarkup);
            base.Walk(nodeTag);

            if (nodeTag.IsMultiLine)
            {
                _classBuilder.AppendNewLine();
                _classBuilder.AppendFormat(nodeTag.Indent + "</{0}>", nodeTag.NamespaceQualifiedTagName);
            }
            else
            {
                _classBuilder.AppendFormat("</{0}>", nodeTag.NamespaceQualifiedTagName);
            }
        }

        private string GetAttributes(IEnumerable<KeyValuePair<string, string>> attributes)
        {
            if (attributes.Count() == 0) return "";

            return " " + string.Join(" ", 
                attributes.Select(x => string.Format("{0}='{1}'", x.Key, x.Value)).ToArray());
        }
    }
}
