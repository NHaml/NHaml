using System;
using System.Collections.Generic;
using NHaml4.Crosscutting;
using NHaml4.Parser;
using NHaml4.IO;
using NHaml4.Compilers;
using NHaml4.TemplateResolution;
using NHaml4.Walkers.CodeDom;

namespace NHaml4
{
    public class TemplateEngine
    {
        private readonly IHamlTemplateCache _compiledTemplateCache;
        private readonly ITemplateFactoryFactory _templateFactoryFactory;

        public TemplateEngine(HamlHtmlOptions htmlOptions)
            : this(
            new SimpleTemplateCache(),
            new TemplateFactoryFactory(
                    new HamlTreeParser(new HamlFileLexer()),
                    new HamlDocumentWalker(new CodeDomClassBuilder(), htmlOptions),
                    new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()),
                    new List<string>(),
                    new List<string>()))
        { }

        public TemplateEngine(IHamlTemplateCache templateCache, ITemplateFactoryFactory templateFactoryFactory)
        {
            _compiledTemplateCache = templateCache;
            _templateFactoryFactory = templateFactoryFactory;
        }

        public TemplateFactory GetCompiledTemplate(ITemplateContentProvider contentProvider, string templatePath, Type templateBaseType)
        {
            Invariant.ArgumentNotNull(contentProvider, "contentProvider");

            var viewSourceCollection = new ViewSourceCollection { contentProvider.GetViewSource(templatePath) };
            return GetCompiledTemplate(viewSourceCollection, templateBaseType);
        }

        public TemplateFactory GetCompiledTemplate(ITemplateContentProvider contentProvider, string templatePath, string masterPath, Type templateBaseType)
        {
            Invariant.ArgumentNotNull(contentProvider, "contentProvider");

            var viewSourceCollection = GetViewSourceCollection(contentProvider, templatePath, masterPath);
            return GetCompiledTemplate(viewSourceCollection, templateBaseType);
        }

        private static ViewSourceCollection GetViewSourceCollection(ITemplateContentProvider contentProvider, string templatePath, string masterPath)
        {
            return new ViewSourceCollection {
                contentProvider.GetViewSource(masterPath),
                contentProvider.GetViewSource(templatePath)
            };
        }

        public TemplateFactory GetCompiledTemplate(IViewSource viewSource, Type templateBaseType)
        {
            return GetCompiledTemplate(new ViewSourceCollection { viewSource }, templateBaseType);
        }

        public TemplateFactory GetCompiledTemplate(ViewSourceCollection viewSourceCollection, Type templateBaseType)
        {
            Invariant.ArgumentNotNull(viewSourceCollection, "viewSourceCollection");
            Invariant.ArgumentNotNull(templateBaseType, "templateBaseType");

            templateBaseType = ProxyExtracter.GetNonProxiedType(templateBaseType);
            var className = viewSourceCollection.GetClassName();

            lock( _compiledTemplateCache )
            {
                return _compiledTemplateCache.GetOrAdd(className, viewSourceCollection[0].TimeStamp,
                    () => _templateFactoryFactory.CompileTemplateFactory(className, viewSourceCollection, templateBaseType));
            }
        }
    }
}