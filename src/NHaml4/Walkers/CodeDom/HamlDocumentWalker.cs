using NHaml4.Compilers;
using NHaml4.Parser;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlDocumentWalker : IDocumentWalker
    {
        private readonly ITemplateClassBuilder _classBuilder;

        public HamlDocumentWalker(ITemplateClassBuilder classBuilder)
        {   
            _classBuilder = classBuilder;
        }
            
        public string Walk(HamlDocument hamlDocument, string className)
        {
            foreach (var child in hamlDocument.Children)
            {
                if (child is HamlNodeText)
                    new HamlNodeTextWalker().Walk(child, _classBuilder);
            }
            return _classBuilder.Build(className);
        }

        private void WalkText(HamlNodeText child)
        {
            _classBuilder.AppendLine("<div>" + child.Text + "</div>");
        }
    }
}
