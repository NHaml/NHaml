using NHaml4.Compilers;
using NHaml4.Parser;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlDocumentWalker : HamlNodeWalker, IDocumentWalker
    {
        public HamlDocumentWalker(ITemplateClassBuilder classBuilder)
            : base (classBuilder, new HamlOptions())
        { }

        public HamlDocumentWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public string Walk(HamlDocument document, string className)
        {
            base.Walk(document);
            return _classBuilder.Build(className);
        }    
    }
}
