using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Configuration;
using NHaml.TemplateResolution;
using NHaml.Utils;

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

     

        public CompiledTemplate Compile(params string[] templatePath )
        {
            return Compile( templatePath, Options.TemplateBaseType );
        }
        public CompiledTemplate Compile( string templatePath )
        {
            return Compile( templatePath, Options.TemplateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, Type templateBaseType )
        {
            return Compile(new List<string>{templatePath}, templateBaseType );
        }


        public CompiledTemplate Compile(List<string> templatePaths )
        {
            return Compile(templatePaths, Options.TemplateBaseType );
        }

        public CompiledTemplate Compile(IList<string> templatePaths, Type templateBaseType)
        {
            var list = ConvertPathsToViewSources(templatePaths);

            return Compile(list, templateBaseType);
        }

        public List<IViewSource> ConvertPathsToViewSources(IList<string> templatePaths)
        {
            var list = new List<IViewSource>();
            foreach (var layoutTemplatePath in templatePaths)
            {
                list.Add(Options.TemplateContentProvider.GetViewSource(layoutTemplatePath));
            }
            return list;
        }


        public CompiledTemplate Compile(IList<IViewSource> templateViewSources, Type templateBaseType )
        {
            return Compile(templateBaseType, templateViewSources, null);
        }

        public CompiledTemplate Compile(Type templateBaseType, IList<IViewSource> templateViewSources, object context)
        {
            Invariant.ArgumentNotNull( templateBaseType, "templateBaseType" );

            templateBaseType = ProxyExtracter.GetNonProxiedType(templateBaseType);
            var templateCacheKey = new StringBuilder();

            foreach( var layoutTemplatePath in templateViewSources )
            {
                templateCacheKey.AppendFormat( "{0}, ", layoutTemplatePath.Path );
            }

            CompiledTemplate compiledTemplate;

            lock( _compiledTemplateCache )
            {
                var key = templateCacheKey.ToString();
                if( !_compiledTemplateCache.TryGetValue( key, out compiledTemplate ) )
                {
                    compiledTemplate = new CompiledTemplate( Options, templateViewSources, templateBaseType,context );
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