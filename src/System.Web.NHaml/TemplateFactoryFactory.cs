using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.TemplateResolution;
using System.Web.NHaml.Walkers;

namespace System.Web.NHaml
{
    public class TemplateFactoryFactory : ITemplateFactoryFactory
    {
        private readonly ITreeParser _treeParser;
        private readonly IDocumentWalker _treeWalker;
        private readonly ITemplateFactoryCompiler _templateFactoryCompiler;

        private readonly IEnumerable<string> _imports;
        private readonly IEnumerable<string> _referencedAssemblyLocations;
        private readonly IDictionary<string, HamlDocument> _hamlDocumentCache = new Dictionary<string, HamlDocument>();
        private ITemplateContentProvider _contentProvider;

        public TemplateFactoryFactory(ITemplateContentProvider contentProvider, ITreeParser treeParser, IDocumentWalker treeWalker,
            ITemplateFactoryCompiler templateCompiler, IEnumerable<string> imports, IEnumerable<string> referencedAssemblyLocations)
        {
            _contentProvider = contentProvider;
            _treeParser = treeParser;
            _treeWalker = treeWalker;
            _templateFactoryCompiler = templateCompiler;
            _imports = imports;
            _referencedAssemblyLocations = referencedAssemblyLocations;
        }

        public TemplateFactory CompileTemplateFactory(string className, ViewSource viewSource)
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

            var masterPage = GetMasterPage();
            hamlDocument = ApplyMasterPage(hamlDocument, masterPage);

            HamlNodePartial partial;
            while ((partial = hamlDocument.GetNextUnresolvedPartial()) != null)
            {
                ViewSource viewSource = GetPartial(viewSourceList, partial.Content);

                var partialDocument = HamlDocumentCacheGetOrAdd(viewSource.FileName,
                    () => _treeParser.ParseViewSource(viewSource));
                partial.SetDocument(partialDocument);
            }
            return hamlDocument;
        }

        // TODO - Missing tests
        private HamlDocument GetMasterPage()
        {
            try
            {
                var masterView = _contentProvider.GetViewSource("Views/Shared/Application.haml");
                return masterView != null
                    ? HamlDocumentCacheGetOrAdd(masterView.FileName, () => _treeParser.ParseViewSource(masterView))
                    : null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        // TODO - Missing tests
        private HamlDocument ApplyMasterPage(HamlDocument hamlDocument, HamlDocument masterPage)
        {
            if (masterPage == null) return hamlDocument;

            HamlNodePartial partial = masterPage.GetNextUnresolvedPartial();
            partial.SetDocument(hamlDocument);
            return masterPage;
        }

        private ViewSource GetPartial(ViewSourceCollection viewSourceList, string partialName)
        {
            ViewSource viewSource = viewSourceList.GetByPartialName(partialName)
                ?? _contentProvider.GetViewSource(partialName);

            if (viewSource == null)
                throw new PartialNotFoundException(partialName);
            return viewSource;
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


        public ITemplateContentProvider TemplateContentProvider
        {
            set { _contentProvider = value; }
        }
    }
}