using NHaml4.Compilers;
using NHaml4.Parser;
using System;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlDocumentWalker : HamlNodeWalker, IDocumentWalker
    {
        public HamlDocumentWalker(ITemplateClassBuilder classBuilder)
            : base (classBuilder, new HamlHtmlOptions())
        { }

        public HamlDocumentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public string Walk(HamlDocument document, string className, Type baseType)
        {
            base.Walk(document);
            return ClassBuilder.Build(className, baseType);
        }    
    }
}
