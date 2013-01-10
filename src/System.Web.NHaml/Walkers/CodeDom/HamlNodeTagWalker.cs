using System.Collections.Generic;
using System.Linq;
using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;

namespace System.Web.NHaml.Walkers.CodeDom
{
    public class HamlNodeTagWalker : HamlNodeWalker
    {
        public HamlNodeTagWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
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
            MakeClassAttribute(nodeTag);
            MakeIdAttribute(nodeTag);
            WalkHtmlStyleAttributes(nodeTag);
        }

        private void MakeClassAttribute(HamlNodeTag nodeTag)
        {
            var classValues = GetClassValues(nodeTag);
            AppendClassAttribute(classValues);
        }

        private static IList<HamlNodeTextContainer> GetClassValues(HamlNodeTag nodeTag)
        {
            var classValues = (from collection in nodeTag.Children.OfType<HamlNodeHtmlAttributeCollection>()
                               from attr in collection.Children.OfType<HamlNodeHtmlAttribute>()
                               where attr.Name == "class"
                               from attrFragment in attr.Children
                               select (HamlNodeTextContainer)attrFragment).ToList();

            classValues.AddRange(nodeTag.Children.OfType<HamlNodeTagClass>().Select(x => new HamlNodeTextContainer(x.SourceFileLineNum, x.Content)));
            return classValues;
        }

        private void AppendClassAttribute(IList<HamlNodeTextContainer> classTextContainers)
        {
            if (!classTextContainers.Any()) return;

            var classFragments = new List<HamlNode>();
            for (int index = 0; index < classTextContainers.Count; index++)
            {
                if (index > 0) classFragments.Add(new HamlNodeTextLiteral(-1, " "));
                classFragments.AddRange(classTextContainers[index].Children);
            }

            ClassBuilder.AppendAttributeNameValuePair("class", classFragments, '\'');
        }

        private void MakeIdAttribute(HamlNodeTag nodeTag)
        {
            var idValues = GetIdValues(nodeTag);
            AppendIdAttribute(idValues);
        }

        private static IList<HamlNodeTextContainer> GetIdValues(HamlNodeTag nodeTag)
        {
            var idValues = (from collection in nodeTag.Children.OfType<HamlNodeHtmlAttributeCollection>()
                            from attr in collection.Children.OfType<HamlNodeHtmlAttribute>()
                            where attr.Name == "id"
                            from attrFragment in attr.Children
                            select (HamlNodeTextContainer)attrFragment).ToList();

            var idTag = nodeTag.Children.LastOrDefault(x => x.GetType() == typeof(HamlNodeTagId));
            if (idTag != null) idValues.Insert(0, new HamlNodeTextContainer(idTag.SourceFileLineNum, idTag.Content));
            return idValues;
        }

        private void AppendIdAttribute(IList<HamlNodeTextContainer> idValues)
        {
            if (!idValues.Any()) return;

            var idFragments = new List<HamlNode>();
            for (int index = 0; index < idValues.Count; index++)
            {
                if (index > 0) idFragments.Add(new HamlNodeTextLiteral(-1, "_"));
                idFragments.AddRange(idValues[index].Children);
            }

            ClassBuilder.AppendAttributeNameValuePair("id", idFragments, '\'');
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
                ClassBuilder.AppendSelfClosingTagSuffix();
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
            if (nodeTag.IsMultiLine == false || nodeTag.WhitespaceRemoval == WhitespaceRemoval.Internal)
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
