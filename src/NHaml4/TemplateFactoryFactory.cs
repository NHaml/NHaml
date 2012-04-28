using System;
using System.Collections.Generic;
using NHaml4.TemplateResolution;
using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Walkers;
using System.Linq;
using NHaml4.Parser.Rules;

namespace NHaml4
{
    public class TemplateFactoryFactory : ITemplateFactoryFactory
    {
        private readonly ITreeParser _treeParser;
        private readonly IDocumentWalker _treeWalker;
        private readonly ITemplateFactoryCompiler _templateFactoryCompiler;
        private readonly IEnumerable<string> _imports;
        private readonly IEnumerable<string> _referencedAssemblyLocations;
        private readonly IDictionary<string, HamlDocument> _hamlDocumentCache = new Dictionary<string, HamlDocument>();

        public TemplateFactoryFactory(ITreeParser treeParser, IDocumentWalker treeWalker,
            ITemplateFactoryCompiler templateCompiler, IEnumerable<string> imports, IEnumerable<string> referencedAssemblyLocations)
        {
            _treeParser = treeParser;
            _treeWalker = treeWalker;
            _templateFactoryCompiler = templateCompiler;
            _imports = imports;
            _referencedAssemblyLocations = referencedAssemblyLocations;
        }

        public TemplateFactory CompileTemplateFactory(string className, IViewSource viewSource)
        {
            return CompileTemplateFactory(className, new ViewSourceCollection { viewSource }, typeof(TemplateBase.Template));
        }

        public TemplateFactory CompileTemplateFactory(string className, ViewSourceCollection viewSourceList)
        {
            return CompileTemplateFactory(className, viewSourceList, typeof(TemplateBase.Template));
        }

        public TemplateFactory CompileTemplateFactory(string className, ViewSourceCollection viewSourceList, Type baseType)
        {
            var hamlDocument = BuildHamlDocument(viewSourceList);
            string templateCode = _treeWalker.Walk(hamlDocument, className, baseType, _imports);
            var templateFactory = _templateFactoryCompiler.Compile(templateCode, className, _referencedAssemblyLocations);
            return templateFactory;
        }

        public HamlDocument BuildHamlDocument(ViewSourceCollection viewSourceList)
        {
            _hamlDocumentCache.Clear();
            var hamlDocument = HamlDocumentCacheGetOrAdd(viewSourceList.First().FileName,
                () => _treeParser.ParseViewSource(viewSourceList.First()));

            HamlNodePartial partial;
            while ((partial = hamlDocument.GetNextUnresolvedPartial()) != null)
            {
                var viewSource = string.IsNullOrEmpty(partial.Content)
                    ? viewSourceList[1]
                    : viewSourceList.GetByPartialName(partial.Content);
                var partialDocument = HamlDocumentCacheGetOrAdd(viewSource.FileName,
                    () => _treeParser.ParseViewSource(viewSource));
                partial.SetDocument(partialDocument);
            }
            return hamlDocument;
        }

        private HamlDocument HamlDocumentCacheGetOrAdd(string key, Func<HamlDocument> getter)
        {
            HamlDocument result;
            bool templateInCache = _hamlDocumentCache.TryGetValue(key, out result);
            return templateInCache
                ? result
                : GetAndAddToCache(key, getter);
        }

        private HamlDocument GetAndAddToCache(string key, Func<HamlDocument> getter)
        {
            var result = getter();
            _hamlDocumentCache[key] = result;
            return result;
        }
    }
}