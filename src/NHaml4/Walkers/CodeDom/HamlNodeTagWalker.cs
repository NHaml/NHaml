using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Crosscutting;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTagWalker : INodeWalker
    {
        private readonly HamlOptions _options;
        private readonly ITemplateClassBuilder _classBuilder;

        public HamlNodeTagWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
        {
            Invariant.ArgumentNotNull(options, "options");
            Invariant.ArgumentNotNull(classBuilder, "classBuilder");
            _options = options;
            _classBuilder = classBuilder;
        }

        public void Walk(HamlNode node)
        {
            var nodeTag = node as HamlNodeTag;
            if (nodeTag == null)
                throw new InvalidCastException("HamlNodeTagWalker requires that HamlNode object be of type HamlNodeTag.");

            string attributes = GetAttributes(nodeTag.Attributes);
            string tagFormat = GetTagFormat(nodeTag);
            _classBuilder.AppendFormat(tagFormat, nodeTag.NamespaceQualifiedTagName, attributes);
        }

        private string GetTagFormat(HamlNodeTag nodeTag)
        {
            if (nodeTag.IsSelfClosing || _options.IsAutoClosingTag(nodeTag.TagName))
                return _options.HtmlVersion == HtmlVersion.XHtml ? "<{0}{1} />" : "<{0}{1}>";
            else
                return "<{0}{1}></{0}>";
        }

        private string GetAttributes(IEnumerable<KeyValuePair<string, string>> attributes)
        {
            if (attributes.Count() == 0) return "";

            return " " + string.Join(" ", 
                attributes.Select(x => string.Format("{0}='{1}'", x.Key, x.Value)).ToArray());
        }
    }
}
