using System;
using System.Collections.Generic;
using System.Text;
using NHaml4.TemplateResolution;
using NHaml.Utils;
using NHaml4.Parser;
using NHaml.IO;
using NHaml4;
using NHaml4.Compilers;
using NHaml4.Compilers.CSharp2;

namespace NHaml
{
    public  class TemplateEngine
    {
        private readonly Dictionary<string, TemplateFactory> _compiledTemplateCache;
        private TemplateFactoryFactory _templateFactoryFactory;

        public TemplateEngine()
        {
            _templateFactoryFactory = new TemplateFactoryFactory(
                new HamlTreeParser(new HamlFileLexer()),
                new NHaml4.Walkers.CodeDom.HamlDocumentWalker(new CSharp2TemplateClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()));
            _compiledTemplateCache = new Dictionary<string, TemplateFactory>();
        }

        public TemplateFactory GetCompiledTemplate(ViewSourceList viewSourceList)
        {
            return GetCompiledTemplate(viewSourceList, typeof(NHaml4.TemplateBase.Template));
        }

        public TemplateFactory GetCompiledTemplate(ViewSourceList viewSourceList, Type templateBaseType)
        {
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
                    //compiledTemplate.Compile();
                    _compiledTemplateCache.Add( key, compiledTemplate );
                    return compiledTemplate;
                }
            }

            return compiledTemplate;
        }
    }
}