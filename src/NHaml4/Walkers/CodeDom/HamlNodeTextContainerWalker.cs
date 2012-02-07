using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextContainerWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeTextContainerWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextContainer;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextWalker requires that HamlNode object be of type HamlNodeText.");

            if (node.IsLeadingWhitespaceTrimmed == false
                && (node.IsWhitespaceNode() == false || node.IsTrailingWhitespaceTrimmed == false))
                ClassBuilder.Append(nodeText.Indent);

            base.Walk(node);
        }
    }
}
