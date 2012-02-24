using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextLiteralWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeTextLiteralWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextLiteral;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextLiteralWalker requires that HamlNode object be of type HamlNodeTextLiteral.");

            string outputText = node.Content;
            if (node.Parent.IsLeadingWhitespaceTrimmed)
                outputText = outputText.TrimStart(new[] { ' ', '\n', '\r', '\t' });
            if (node.Parent.IsTrailingWhitespaceTrimmed)
                outputText = outputText.TrimEnd(new[] { ' ', '\n', '\r', '\t' });

            if (outputText.Length > 0)
            {
                ClassBuilder.Append(outputText);
            }
        }
    }
}
