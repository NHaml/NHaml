using System;
using System.Collections.Generic;
using NHaml4.TemplateResolution;
using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Walkers;

namespace NHaml4
{
    public class TemplateFactoryFactory : ITemplateFactoryFactory
    {
        private readonly ITreeParser _treeParser;
        private readonly IDocumentWalker _treeWalker;
        private readonly ITemplateFactoryCompiler _templateFactoryCompiler;

        public TemplateFactoryFactory(ITreeParser treeParser,
            IDocumentWalker treeWalker, ITemplateFactoryCompiler templateCompiler)
        {
            _treeParser = treeParser;
            _treeWalker = treeWalker;
            _templateFactoryCompiler = templateCompiler;
        }

        public TemplateFactory CompileTemplateFactory(string className, IViewSource viewSource)
        {
            var hamlDocument = _treeParser.ParseViewSource(viewSource);
            string templateCode = _treeWalker.Walk(hamlDocument, className);
            var templateFactory = _templateFactoryCompiler.Compile(templateCode, className, GetCompileTypes() );
            return templateFactory;
        }

        private IList<Type> GetCompileTypes()
        {
            return new List<Type> {
                typeof(TemplateBase.Template),
                typeof(System.Web.HttpUtility)
            };
        }
    }
}