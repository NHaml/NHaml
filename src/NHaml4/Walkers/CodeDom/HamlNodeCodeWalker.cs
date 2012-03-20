using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeCodeWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeCodeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeEval = node as HamlNodeCode;
            if (nodeEval == null)
                throw new System.InvalidCastException("HamlNodeCode requires that HamlNode object be of type HamlNodeCode.");

            //if (node.IsLeadingWhitespaceTrimmed == false)
            //    ClassBuilder.Append(node.Indent);
            ClassBuilder.AppendCodeSnippet(node.Content);
            
            base.Walk(node);
        }
    }
}
