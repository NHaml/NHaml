using NHaml4.Parser;
using NHaml4.Compilers;
namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextWalker : INodeWalker
    {
        public void Walk(HamlNode node, ITemplateClassBuilder classBuilder)
        {
            var nodeText = node as HamlNodeText;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextWalker requires that HamlNode object be of type HamlNodeText.");

            classBuilder.Append(nodeText.Text);
        }
    }
}
