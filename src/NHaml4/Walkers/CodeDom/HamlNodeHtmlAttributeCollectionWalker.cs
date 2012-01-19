using NHaml4.Compilers;
using NHaml4.Parser.Rules;
using NHaml4.Crosscutting;
using NHaml4.Parser;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeHtmlAttributeCollectionWalker : HamlNodeWalker
    {
        public HamlNodeHtmlAttributeCollectionWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(Parser.HamlNode node)
        {
            var attributeCollectionNode = node as HamlNodeHtmlAttributeCollection;
            if (attributeCollectionNode == null)
                throw new System.InvalidCastException("HamlNodeHtmlAttributeCollectionWalker requires that HamlNode object be of type HamlNodeHtmlAttributeCollection.");

            foreach (HamlNodeHtmlAttribute childNode in attributeCollectionNode.Children)
            {
                if (childNode.Content.StartsWith("class=")
                    || childNode.Content.StartsWith("id=")) continue;
                _classBuilder.Append(MakeAttribute(childNode));
            }
        }

        private string MakeAttribute(HamlNode childNode)
        {
            var attributeNode = childNode as HamlNodeHtmlAttribute;
            if (attributeNode == null)
                throw new HamlMalformedTagException("Unexpected " + childNode.GetType().FullName + " tag in AttributeCollection node",
                    childNode.SourceFileLineNo);

            if ((string.IsNullOrEmpty(attributeNode.Name)) || (attributeNode.Value == "false"))
                return "";
            if ((attributeNode.Value == "true") || (attributeNode.Value == ""))
            {
                if (_options.HtmlVersion == HtmlVersion.XHtml)
                    return " " + attributeNode.Name + "='" + attributeNode.Name + "'";
                else
                    return " " + attributeNode.Name;
            }
            return " " + attributeNode.Name + "=" + attributeNode.Value;
        }
    }
}
