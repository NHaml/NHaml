using System;
using System.Collections.Generic;
using NHaml4.Crosscutting;
using NHaml4.Parser;
using NHaml4.IO;
using NHaml4.Compilers;
using NHaml4.Compilers.CSharp2;
using NHaml4.TemplateResolution;
using NHaml4.Walkers.CodeDom;
using NHaml4.Compilers.Abstract;

namespace NHaml4
{
    public class TemplateEngine
    {
        private readonly IHamlTemplateCache _compiledTemplateCache;
        private readonly ITemplateFactoryFactory _templateFactoryFactory;

        public TemplateEngine(HamlOptions hamlOptions)
            : this(
            new SimpleTemplateCache(),
            new TemplateFactoryFactory(
                    new HamlTreeParser(new HamlFileLexer()),
                    new HamlDocumentWalker(new CodeDomClassBuilder(), hamlOptions),
                    new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder())))
        { }

        public TemplateEngine(IHamlTemplateCache templateCache, ITemplateFactoryFactory templateFactoryFactory)
        {
            _templateFactoryFactory = templateFactoryFactory;
            _compiledTemplateCache = templateCache;
        }

        public TemplateFactory GetCompiledTemplate(ITemplateContentProvider contentProvider, string templatePath, Type baseType)
        {
            Invariant.ArgumentNotNull(contentProvider, "contentProvider");

            var viewSource = contentProvider.GetViewSource(templatePath);
            return GetCompiledTemplate(viewSource, baseType);
        }

        public TemplateFactory GetCompiledTemplate(IViewSource viewSource)
        {
            return GetCompiledTemplate(viewSource, typeof(TemplateBase.Template));
        }

        public TemplateFactory GetCompiledTemplate(IViewSource viewSource, Type templateBaseType)
        {
            Invariant.ArgumentNotNull(viewSource, "viewSource");
            Invariant.ArgumentNotNull(templateBaseType, "templateBaseType");

            templateBaseType = ProxyExtracter.GetNonProxiedType(templateBaseType);
            var className = viewSource.GetClassName();

            lock( _compiledTemplateCache )
            {
                return _compiledTemplateCache.GetOrAdd(className, viewSource.TimeStamp,
                    () => _templateFactoryFactory.CompileTemplateFactory(className, viewSource));
            }
        }
    }
}