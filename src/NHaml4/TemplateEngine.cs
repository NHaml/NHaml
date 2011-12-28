using System;
using System.Collections.Generic;
using NHaml4.Crosscutting;
using NHaml4.Parser;
using NHaml4.IO;
using NHaml4.Compilers;
using NHaml4.Compilers.CSharp2;
using NHaml4.TemplateResolution;
using NHaml4.Walkers.CodeDom;

namespace NHaml4
{
    public  class TemplateEngine
    {
        private readonly Dictionary<string, TemplateFactory> _compiledTemplateCache;
        private readonly ITemplateFactoryFactory _templateFactoryFactory;

        public TemplateEngine(HamlOptions hamlOptions)
            : this(new TemplateFactoryFactory(
                    new HamlTreeParser(new HamlFileLexer()),
                    new HamlDocumentWalker(new CSharp2TemplateClassBuilder(), hamlOptions),
                    new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder())))
        { }

        public TemplateEngine(ITemplateFactoryFactory templateFactoryFactory)
        {
            _templateFactoryFactory = templateFactoryFactory;
            _compiledTemplateCache = new Dictionary<string, TemplateFactory>();
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
                
            TemplateFactory compiledTemplate;

            lock( _compiledTemplateCache )
            {
                if (!_compiledTemplateCache.TryGetValue(className, out compiledTemplate))
                {
                    compiledTemplate = _templateFactoryFactory.CompileTemplateFactory(className, viewSource);
                    _compiledTemplateCache.Add(className, compiledTemplate);
                    return compiledTemplate;
                }
            }

            return compiledTemplate;
        }
    }
}