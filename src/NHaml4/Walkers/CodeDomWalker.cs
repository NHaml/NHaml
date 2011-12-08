using NHaml4.Compilers;
using NHaml4.Parser;

namespace NHaml4.Walkers
{
    public class CodeDomWalker : IHamlTreeWalker
    {
        private readonly ITemplateClassBuilder _classBuilder;

        public CodeDomWalker(ITemplateClassBuilder classBuilder)
        {   
            _classBuilder = classBuilder;
        }
            
        public string Walk(HamlDocument hamlDocument, string className)
        {
            foreach (var child in hamlDocument.Children)
            {
                if (child is HamlNodeText)
                    WalkText((HamlNodeText)child);
            }
            return _classBuilder.Build(className);
        }

        private void WalkText(HamlNodeText child)
        {
            _classBuilder.AppendLine("<div>" + child.Text + "</div>");
        }
    }
}
