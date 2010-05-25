using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.TemplateResolution;
using NHaml.Core.Utils;
using NHaml.Core.Configuration;
using NHaml.Core.Ast;
using NHaml.Core.Visitors;

namespace NHaml.Core.Template
{
    public class TemplateEngine
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

        public CompiledTemplate Compile( string templatePath )
        {
            return Compile( Options.TemplateContentProvider.GetViewSource(templatePath), null );
        }

        public CompiledTemplate Compile( string templatePath, string masterPath )
        {
            return Compile( Options.TemplateContentProvider.GetViewSource(templatePath),
                            Options.TemplateContentProvider.GetViewSource(masterPath)
                          );
        }

        public CompiledTemplate Compile(string templatePath, string masterPath, string defaultMasterPath)
        {
            return Compile(Options.TemplateContentProvider.GetViewSource(templatePath),
                            Options.TemplateContentProvider.GetViewSource(masterPath),
                            Options.TemplateContentProvider.GetViewSource(defaultMasterPath)
                          );
        }

        public CompiledTemplate Compile(IViewSource template, IViewSource master)
        {
            return Compile(template, master, null);
        }

        public CompiledTemplate Compile(IViewSource template, IViewSource master, IViewSource defaultMaster)
        {
            return Compile(template,
                Compile(master, (IViewSource)null, null),
                Compile(defaultMaster, (IViewSource)null, null),
                null);
        }

        public CompiledTemplate Compile(IViewSource template, CompiledTemplate master, CompiledTemplate defaultMaster, object context)
        {
            Invariant.ArgumentNotNull( template, "template" );

            var opts = (TemplateOptions)Options.Clone();

            MetaNode pagedefiniton = MetaDataFiller.FillAndGetPageDefinition(template.ParseResult.Metadata, Options);
            // inherittype will contain the @type data too
            string inherittype = ((TextChunk)((TextNode)pagedefiniton.Attributes.Find(x => x.Name == "Inherits").Value).Chunks[0]).Text;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var modelType = assembly.GetType(inherittype, false, true);
                if (modelType != null)
                {
                    opts.TemplateBaseType = ProxyExtractor.GetNonProxiedType(modelType);
                }
            }
            
            var templateCacheKey = template.Path;

            // check if there is a default masterpagefile definition inside the content page.
            // If yes, use that instead of the defaultTemplate
            if (master == null)
            {
                AttributeNode masterNode = pagedefiniton.Attributes.Find(x => x.Name == "MasterPageFile");
                if (masterNode != null)
                {
                    string masterName = ((TextChunk)((TextNode)masterNode.Value).Chunks[0]).Text;
                    master = Compile(masterName);
                }
                else
                {
                    if (defaultMaster != null)
                    {
                        master = defaultMaster;
                    }
                }
            }

            CompiledTemplate compiledTemplate;

            lock( _compiledTemplateCache )
            {
                var key = templateCacheKey.ToString();
                if( !_compiledTemplateCache.TryGetValue( key, out compiledTemplate ) )
                {
                    compiledTemplate = new CompiledTemplate(opts, opts.TemplateBaseType, context, master, template);
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