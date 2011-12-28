using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextWalker : INodeWalker
    {
        private readonly HamlOptions _options;
        private readonly ITemplateClassBuilder _classBuilder;

        public HamlNodeTextWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
        {
            Invariant.ArgumentNotNull(options, "options");
            Invariant.ArgumentNotNull(classBuilder, "classBuilder");
            _options = options;
            _classBuilder = classBuilder;
        }

        public void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeText;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextWalker requires that HamlNode object be of type HamlNodeText.");

            _classBuilder.Append(nodeText.Text);
        }
    }
}
