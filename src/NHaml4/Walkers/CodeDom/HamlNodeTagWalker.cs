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
            if (nodeTag.IsLeadingWhitespaceTrimmed == false)
                ClassBuilder.Append(nodeTag.Indent);

            ClassBuilder.Append("<" + nodeTag.NamespaceQualifiedTagName);
        }

        private void AppendAttributes(HamlNodeTag nodeTag)
        {
            ClassBuilder.Append(MakeClassAttribute(nodeTag));
            ClassBuilder.Append(MakeIdAttribute(nodeTag));
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
                new HamlNodeHtmlAttributeCollectionWalker(ClassBuilder, Options)
                    .Walk(child);
            }
        }

        private void AppendTagBodyAndClose(HamlNodeTag nodeTag)
        {
            if (nodeTag.IsSelfClosing || Options.IsAutoClosingTag(nodeTag.TagName))
                ClassBuilder.Append(Options.HtmlVersion == HtmlVersion.XHtml ?
                    " />"
                    : ">");
            else
            {
                ClassBuilder.Append(">");
                base.Walk(nodeTag);
                if (IsPreCloseTagWhitespaceTrimmed(nodeTag))
                {
                    ClassBuilder.AppendFormat("</{0}>", nodeTag.NamespaceQualifiedTagName);
                }
                else
                {
                    ClassBuilder.AppendNewLine();
                    ClassBuilder.AppendFormat(nodeTag.Indent + "</{0}>", nodeTag.NamespaceQualifiedTagName);
                }
            }
        }

        private bool IsPreCloseTagWhitespaceTrimmed(HamlNodeTag nodeTag)
        {
            if (nodeTag.IsMultiLine == false)
                return true;
            else if (nodeTag.WhitespaceRemoval == WhitespaceRemoval.Internal)
                return true;

            var lastNonWhitespaceChild = GetLastNonWhitespaceChild(nodeTag) as HamlNodeTag;
            if (lastNonWhitespaceChild == null)
                return false;
            
            return (lastNonWhitespaceChild).WhitespaceRemoval == WhitespaceRemoval.Surrounding;
        }

        private HamlNode GetLastNonWhitespaceChild(HamlNodeTag nodeTag)
        {
            return nodeTag.Children.LastOrDefault(x => x.IsWhitespaceNode() == false);
        }
    }
}
