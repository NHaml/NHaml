using System;
using System.Collections.Generic;
using NHaml4.TemplateResolution;
using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Walkers;

namespace NHaml4
{
    public class TemplateFactoryFactory
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

        public TemplateFactory CompileTemplateFactory(IViewSource viewSource)
        {
            return CompileTemplateFactory(new ViewSourceList { viewSource });
        }

        public TemplateFactory CompileTemplateFactory(ViewSourceList viewSourceList)
        {
            string className = viewSourceList.GetClassNameFromPathName();
            var hamlDocument = _treeParser.ParseViewSources(viewSourceList);
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