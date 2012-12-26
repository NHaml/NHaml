using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;

namespace System.Web.NHaml.Walkers.CodeDom
{
    public class HamlNodeTextContainerWalker : HamlNodeWalker
    {
        public HamlNodeTextContainerWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextContainer;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextWalker requires that HamlNode object be of type HamlNodeText.");

            RenderIndent(node, nodeText);
            base.Walk(node);
        }

        private void RenderIndent(HamlNode node, HamlNodeTextContainer nodeText)
        {
            if (node.IsLeadingWhitespaceTrimmed) return;
            if (node.IsWhitespaceNode() && node.IsTrailingWhitespaceTrimmed) return;
            
            ClassBuilder.Append(nodeText.Indent);
        }
    }
}
