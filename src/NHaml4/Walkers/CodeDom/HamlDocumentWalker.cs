using NHaml4.Compilers;
using NHaml4.Parser;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlDocumentWalker : IDocumentWalker
    {
        private readonly ITemplateClassBuilder _classBuilder;
        private readonly HamlOptions _options;

        public HamlDocumentWalker(ITemplateClassBuilder classBuilder)
            : this(classBuilder, new HamlOptions())
        { }

        public HamlDocumentWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
        {
            _classBuilder = classBuilder;
            _options = options;
        }

        public string Walk(HamlDocument hamlDocument, string className)
        {
            foreach (var child in hamlDocument.Children)
            {
                if (child is HamlNodeText)
                    new HamlNodeTextWalker(_classBuilder, _options).Walk(child);
                if (child is HamlNodeTag)
                    new HamlNodeTagWalker(_classBuilder, _options).Walk(child);
            }
            return _classBuilder.Build(className);
        }
    }
}
