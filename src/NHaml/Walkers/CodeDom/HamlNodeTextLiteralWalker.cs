using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;

namespace System.Web.NHaml.Walkers.CodeDom
{
    public sealed class HamlNodeTextLiteralWalker : HamlNodeWalker
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
            outputText = HandleLeadingWhitespace(node, outputText);
            outputText = HandleTrailingWhitespace(node, outputText);

            if (outputText.Length > 0)
                ClassBuilder.Append(outputText);
        }

        private static string HandleTrailingWhitespace(HamlNode node, string outputText)
        {
            if (node.Parent.IsTrailingWhitespaceTrimmed)
                outputText = outputText.TrimEnd(new[] { ' ', '\n', '\r', '\t' });
            return outputText;
        }

        private static string HandleLeadingWhitespace(HamlNode node, string outputText)
        {
            if (node.Parent.IsLeadingWhitespaceTrimmed)
                outputText = outputText.TrimStart(new[] { ' ', '\n', '\r', '\t' });
            return outputText;
        }
    }
}
