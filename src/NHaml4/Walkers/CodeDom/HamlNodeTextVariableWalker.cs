using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextVariableWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeTextVariableWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextVariable;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextVariableWalker requires that HamlNode object be of type HamlNodeTextVariable.");

            ClassBuilder.Append(node.Content);
        }
    }
}
