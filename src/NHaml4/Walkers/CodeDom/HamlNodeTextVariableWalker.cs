using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextVariableWalker : HamlNodeWalker
    {
        public HamlNodeTextVariableWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeTextVariable;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextVariableWalker requires that HamlNode object be of type HamlNodeTextVariable.");

            string variableName = node.Content.Substring(2, node.Content.Length-3);

            ClassBuilder.AppendVariable(variableName);
        }
    }
}
