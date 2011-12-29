using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
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

            _classBuilder.Append(nodeText.Text);
        }
    }
}
