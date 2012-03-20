using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeEvalWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeEvalWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeEval = node as HamlNodeEval;
            if (nodeEval == null)
                throw new System.InvalidCastException("HamlNodeEvalWalker requires that HamlNode object be of type HamlNodeEval.");

            ClassBuilder.AppendCodeToString(node.Content);

            base.ValidateThereAreNoChildren(node);
        }
    }
}
