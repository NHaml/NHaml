using System;
using System.Collections.Generic;
using NHaml;
using NHaml.Utils;
using NHaml4.Parser;
using NHaml4.IO;
using NHaml4;
using NHaml4.Compilers;
using NHaml4.Compilers.CSharp2;
using NHaml4.Walkers.CodeDom;

namespace NHaml4
{
    public  class TemplateEngine
    {
        private readonly Dictionary<string, TemplateFactory> _compiledTemplateCache;
        private readonly ITemplateFactoryFactory _templateFactoryFactory;

        public TemplateEngine()
            : this(new TemplateFactoryFactory(
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CSharp2TemplateClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder())))
        { }

        public TemplateEngine(ITemplateFactoryFactory templateFactoryFactory)
        {
            _templateFactoryFactory = templateFactoryFactory;
            _compiledTemplateCache = new Dictionary<string, TemplateFactory>();
        }

        public TemplateFactory GetCompiledTemplate(ViewSourceList viewSourceList)
        {
            return GetCompiledTemplate(viewSourceList, typeof(TemplateBase.Template));
        }

        public TemplateFactory GetCompiledTemplate(ViewSourceList viewSourceList, Type templateBaseType)
        {
            Invariant.ArgumentNotNull(viewSourceList, "viewSourceList");
            Invariant.ArgumentNotNull(templateBaseType, "templateBaseType");

            templateBaseType = ProxyExtracter.GetNonProxiedType(templateBaseType);
            var templateCacheKey = viewSourceList.GetCacheKey();
                
            TemplateFactory compiledTemplate;

            lock( _compiledTemplateCache )
            {
                var key = templateCacheKey.ToString();
                if( !_compiledTemplateCache.TryGetValue( key, out compiledTemplate ) )
                {
                    compiledTemplate = _templateFactoryFactory.CompileTemplateFactory(viewSourceList);
                    _compiledTemplateCache.Add( key, compiledTemplate );
                    return compiledTemplate;
                }
            }

            return compiledTemplate;
        }
    }
}