using System;
using System.Collections.Generic;
using NHaml4.TemplateResolution;
using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Walkers;
using NHaml4.IO;
using NHaml4.Walkers.CodeDom;
using NHaml4.Compilers.Abstract;

namespace NHaml4
{
    public class TemplateFactoryFactory : ITemplateFactoryFactory
    {
        private readonly ITreeParser _treeParser;
        private readonly IDocumentWalker _treeWalker;
        private readonly ITemplateFactoryCompiler _templateFactoryCompiler;

        public TemplateFactoryFactory(Walkers.CodeDom.HamlHtmlOptions hamlOptions)
            : this(hamlOptions, new List<string>(), new List<string>())
        { }

        public TemplateFactoryFactory(Walkers.CodeDom.HamlHtmlOptions hamlOptions, IList<string> imports, IList<string> referencedAssemblyLocations)
            : this(new HamlTreeParser(new HamlFileLexer()),
                    new HamlDocumentWalker(new CodeDomClassBuilder(imports), hamlOptions),
                    new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder(), referencedAssemblyLocations))
        { }

        public TemplateFactoryFactory(ITreeParser treeParser, IDocumentWalker treeWalker,
            ITemplateFactoryCompiler templateCompiler)
        {
            _treeParser = treeParser;
            _treeWalker = treeWalker;
            _templateFactoryCompiler = templateCompiler;
        }

        public TemplateFactory CompileTemplateFactory(string className, IViewSource viewSource)
        {
            return CompileTemplateFactory(className, viewSource, typeof(TemplateBase.Template));
        }

        public TemplateFactory CompileTemplateFactory(string className, IViewSource viewSource, Type baseType)
        {
            var hamlDocument = _treeParser.ParseViewSource(viewSource);
            string templateCode = _treeWalker.Walk(hamlDocument, className, baseType);
            var templateFactory = _templateFactoryCompiler.Compile(templateCode, className);
            return templateFactory;
        }
    }
}