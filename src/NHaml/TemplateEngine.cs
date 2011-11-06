using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Configuration;
using NHaml.TemplateResolution;
using NHaml.Utils;
using NHaml.Parser;

namespace NHaml
{
    public  class TemplateEngine
    {
        private readonly Dictionary<string, CompiledTemplate> _compiledTemplateCache;

        public TemplateEngine()
            : this( new TemplateOptions() )
        {
        }

        public TemplateEngine( TemplateOptions options )
        {
            Invariant.ArgumentNotNull( options, "options" );

            Options = options;

            _compiledTemplateCache = new Dictionary<string, CompiledTemplate>();

            NHamlConfigurationSection.UpdateTemplateOptions( Options );
            Options.TemplateBaseTypeChanged += (sender, args) => ClearCompiledTemplatesCache();
            Options.TemplateCompilerChanged += (sender, args) => ClearCompiledTemplatesCache();
        }

        public TemplateOptions Options { get; private set; }


        private void ClearCompiledTemplatesCache()
        {
            lock( _compiledTemplateCache )
            {
                _compiledTemplateCache.Clear();
                //TODO: perhaps update usings here
            }
        }
        
        public CompiledTemplate GetCompiledTemplate(TemplateCompileResources resources)
        {
            Invariant.ArgumentNotNull(resources.TemplateBaseType, "templateBaseType");

            var templateBaseType = ProxyExtracter.GetNonProxiedType(resources.TemplateBaseType);
            var templateCacheKey = new StringBuilder();

            foreach (var layoutTemplatePath in resources.GetViewSources(Options.TemplateContentProvider))
            {
                templateCacheKey.AppendFormat( "{0}, ", layoutTemplatePath.Path );
            }

            CompiledTemplate compiledTemplate;

            lock( _compiledTemplateCache )
            {
                var key = templateCacheKey.ToString();
                if( !_compiledTemplateCache.TryGetValue( key, out compiledTemplate ) )
                {
                    compiledTemplate = new CompiledTemplate(Options, resources, new HamlTreeParser());
                    compiledTemplate.Compile();
                    _compiledTemplateCache.Add( key, compiledTemplate );
                    return compiledTemplate;
                }
            }

            if( Options.AutoRecompile )
            {
                compiledTemplate.Recompile();
            }

            return compiledTemplate;
        }
    }
}