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

            RenderTagStart(nodeTag);
            WalkAttributes(nodeTag);
            RenderTagBodyAndClose(nodeTag);
        }

        private void RenderTagStart(HamlNodeTag nodeTag)
        {
            _classBuilder.Append(nodeTag.Indent);
            _classBuilder.Append("<" + nodeTag.NamespaceQualifiedTagName);
        }

        private void WalkAttributes(HamlNodeTag nodeTag)
        {
            var classTags = nodeTag.Children.Where(x => x.GetType() == typeof(HamlNodeTagClass));
            if (classTags.Count() > 0)
            {
                _classBuilder.AppendFormat(" class='{0}'",
                    string.Join(" ", classTags.Select(x => x.Content).ToArray()));
            }

            var idTag = nodeTag.Children.LastOrDefault(x => x.GetType() == typeof(HamlNodeTagId));
            if (idTag != null) _classBuilder.AppendFormat(" id='{0}'", idTag.Content);

            //var attributes = nodeTag.Children.Where(x => x.GetType() == typeof(HamlNodeTagAttribute));
            //foreach (var child in nodeTag.Children)
            //{
            //    if (child.GetType() == typeof())
            //}
        }

        private void RenderTagBodyAndClose(HamlNodeTag nodeTag)
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
