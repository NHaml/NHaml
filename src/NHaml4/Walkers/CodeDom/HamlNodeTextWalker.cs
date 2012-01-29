using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeTextWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeText;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextWalker requires that HamlNode object be of type HamlNodeText.");

            string outputText = nodeText.Indent + nodeText.Content;

            if (node.TrimLeadingWhitespace())
                outputText = outputText.TrimStart(new[] { ' ', '\n', '\r', '\t'});
            if (node.TrimTrailingWhitespace())
                outputText = outputText.TrimEnd(new[] { ' ', '\n', '\r', '\t'});

            if (outputText.Length > 0)
            {
                _classBuilder.Append(outputText);
            }
            base.Walk(node);
        }
    }
}
