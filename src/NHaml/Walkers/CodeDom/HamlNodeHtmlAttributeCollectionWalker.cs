using NHaml.Compilers;
using NHaml.Parser.Rules;
using NHaml.Parser;
using NHaml.Parser.Exceptions;
using System.Linq;

namespace NHaml.Walkers.CodeDom
{
    public class HamlNodeHtmlAttributeCollectionWalker : HamlNodeWalker
    {
        public HamlNodeHtmlAttributeCollectionWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var attributeCollectionNode = node as HamlNodeHtmlAttributeCollection;
            if (attributeCollectionNode == null)
                throw new System.InvalidCastException("HamlNodeHtmlAttributeCollectionWalker requires that HamlNode object be of type HamlNodeHtmlAttributeCollection.");

            foreach (HamlNodeHtmlAttribute childNode in attributeCollectionNode.Children)
            {
                if (childNode.Content.StartsWith("class=")
                    || childNode.Content.StartsWith("id=")) continue;
                MakeAttribute(childNode);
            }
        }

        private void MakeAttribute(HamlNode childNode)
        {
            var attributeNode = childNode as HamlNodeHtmlAttribute;
            if (attributeNode == null)
                throw new HamlMalformedTagException("Unexpected " + childNode.GetType().FullName + " tag in AttributeCollection node",
                    childNode.SourceFileLineNum);

            ClassBuilder.AppendAttributeNameValuePair(attributeNode.Name, attributeNode.Children.Select(x => x.Content), attributeNode.QuoteChar);
        }
    }
}
