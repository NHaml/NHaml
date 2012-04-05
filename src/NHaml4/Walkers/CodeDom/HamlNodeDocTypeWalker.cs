using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;
using NHaml4.TemplateBase;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeDocTypeWalker : HamlNodeWalker, INodeWalker
    {
        public class DocTypeInfo
        {
            public string DocType;
            public string DocSubType;
        }

        public HamlNodeDocTypeWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeEval = node as HamlNodeDocType;
            if (nodeEval == null)
                throw new System.InvalidCastException("HamlNodeDocTypeWalker requires that HamlNode object be of type HamlNodeDocType.");

            ClassBuilder.AppendDocType(node.Content.Trim());

            base.ValidateThereAreNoChildren(node);
        }
    }
}
