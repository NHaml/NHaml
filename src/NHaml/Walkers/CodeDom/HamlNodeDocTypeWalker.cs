using NHaml.Parser;
using NHaml.Compilers;
using NHaml.Parser.Rules;

namespace NHaml.Walkers.CodeDom
{
    public sealed class HamlNodeDocTypeWalker : HamlNodeWalker
    {
        public HamlNodeDocTypeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeEval = node as HamlNodeDocType;
            if (nodeEval == null)
                throw new System.InvalidCastException("HamlNodeDocTypeWalker requires that HamlNode object be of type HamlNodeDocType.");

            ClassBuilder.AppendDocType(node.Content.Trim());

            ValidateThereAreNoChildren(node);
        }
    }
}
