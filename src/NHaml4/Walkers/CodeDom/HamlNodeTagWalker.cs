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

            AppendTagStart(nodeTag);
            AppendAttributes(nodeTag);
            AppendTagBodyAndClose(nodeTag);
        }

        private void AppendTagStart(HamlNodeTag nodeTag)
        {
            AppendLeadingWhitespace(nodeTag);
            _classBuilder.Append("<" + nodeTag.NamespaceQualifiedTagName);
        }

        private void AppendLeadingWhitespace(HamlNodeTag nodeTag)
        {
            if (nodeTag.WhitespaceRemoval == WhitespaceRemoval.Surrounding)
                return;

            var previousTag = nodeTag.Previous as HamlNodeTag;
            if (previousTag != null && previousTag.WhitespaceRemoval == WhitespaceRemoval.Surrounding)
                return;

            var parentTag = nodeTag.Parent as HamlNodeTag;
            if (parentTag != null && parentTag.WhitespaceRemoval == WhitespaceRemoval.Internal)
                return;

            _classBuilder.Append(nodeTag.Indent);
        }

        private void AppendAttributes(HamlNodeTag nodeTag)
        {
            _classBuilder.Append(MakeClassAttribute(nodeTag));
            _classBuilder.Append(MakeIdAttribute(nodeTag));
            WalkHtmlStyleAttributes(nodeTag);
        }

        private string MakeClassAttribute(HamlNodeTag nodeTag)
        {
            var classes = (from collection in nodeTag.Children
                           from attr in collection.Children.OfType<HamlNodeHtmlAttribute>()
                           where ((HamlNodeHtmlAttribute)attr).Name == "class"
                           select ((HamlNodeHtmlAttribute)attr).ValueWithoutQuotes).ToList();

            var classesToAdd = nodeTag.Children.OfType<HamlNodeTagClass>()
                .Select(x => x.Content).ToList();

            classes.AddRange(classesToAdd);

            return (classes.Any())
                ? string.Format(" class='{0}'", string.Join(" ", classes.ToArray()))
                : "";
        }

        private string MakeIdAttribute(HamlNodeTag nodeTag)
        {
            var idAttributes = (from collection in nodeTag.Children
                                from attr in collection.Children.OfType<HamlNodeHtmlAttribute>()
                                where ((HamlNodeHtmlAttribute)attr).Name == "id"
                                select ((HamlNodeHtmlAttribute)attr).ValueWithoutQuotes).ToList();

            var idTag = nodeTag.Children.LastOrDefault(x => x.GetType() == typeof(HamlNodeTagId));
            if (idTag != null) idAttributes.Insert(0, idTag.Content);

            return idAttributes.Any() ? " id='" + string.Join("_", idAttributes.ToArray()) + "'" : "";
        }

        private void WalkHtmlStyleAttributes(HamlNodeTag nodeTag)
        {
            var attributeTags = nodeTag.Children.Where(x => x.GetType() == typeof(HamlNodeHtmlAttributeCollection));
            foreach (var child in attributeTags)
            {
                new HamlNodeHtmlAttributeCollectionWalker(_classBuilder, _options)
                    .Walk(child);
            }
        }

        private void AppendTagBodyAndClose(HamlNodeTag nodeTag)
        {
            if (nodeTag.IsSelfClosing || _options.IsAutoClosingTag(nodeTag.TagName))
                _classBuilder.Append(_options.HtmlVersion == HtmlVersion.XHtml ?
                    " />"
                    : ">");
            else
            {
                _classBuilder.Append(">");
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
