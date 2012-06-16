using NHaml4.Compilers;
using NHaml4.Parser;
using System;
using System.Collections.Generic;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlDocumentWalker : HamlNodeWalker, IDocumentWalker
    {
        public HamlDocumentWalker(ITemplateClassBuilder classBuilder)
            : base (classBuilder, new HamlHtmlOptions())
        { }

        public HamlDocumentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions htmlOptions)
            : base(classBuilder, htmlOptions)
        { }

        public string Walk(HamlDocument document, string className, Type baseType, IEnumerable<string> imports)
        {
            ClassBuilder.Clear();
            base.Walk(document);
            return ClassBuilder.Build(className, baseType, imports);
        }    
    }
}
