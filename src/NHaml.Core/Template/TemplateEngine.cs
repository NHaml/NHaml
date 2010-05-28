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
            IViewSource template = Options.TemplateContentProvider.GetViewSource(templatePath);
            IViewSource master = null;
            if (masterPath!=null) {
                master = Options.TemplateContentProvider.GetViewSource(masterPath);
            }
            return Compile(template,master);
        }

        public CompiledTemplate Compile(string templatePath, string masterPath, string defaultMasterPath, Type BaseType)
        {
            IViewSource template = Options.TemplateContentProvider.GetViewSource(templatePath);
            IViewSource master = null;
            if (masterPath != null)
            {
                master = Options.TemplateContentProvider.GetViewSource(masterPath);
            }
            IViewSource defaultMaster = null;
            if (defaultMasterPath != null)
            {
                defaultMaster = Options.TemplateContentProvider.GetViewSource(defaultMasterPath);
            }
            return Compile(template,master,defaultMaster, BaseType);
        }

        public CompiledTemplate Compile(IViewSource template, IViewSource master)
        {
            return Compile(template, master, null, null);
        }

        public CompiledTemplate Compile(IViewSource template, IViewSource master, IViewSource defaultMaster, Type BaseType)
        {
            CompiledTemplate compiledMaster = null;
            if (master != null)
                compiledMaster = Compile(master, (IViewSource)null, null, null);

            CompiledTemplate compiledDefaultMaster = null;
            if (defaultMaster != null)
                compiledDefaultMaster = Compile(defaultMaster, (IViewSource)null, null, null);

            return Compile(template,compiledMaster, compiledDefaultMaster, null, BaseType);
        }

        public CompiledTemplate Compile(IViewSource template, CompiledTemplate master, CompiledTemplate defaultMaster, object context, Type BaseType)
        {
            Invariant.ArgumentNotNull( template, "template" );

            var opts = (TemplateOptions)Options.Clone();
            var origtype = opts.TemplateBaseType;
            if (BaseType != null)
            {
                opts.TemplateBaseType = BaseType;
            }

            List<MetaNode> data;
            if (template.ParseResult.Metadata.TryGetValue("assembly", out data))
            {
                foreach (var assemblyNode in data)
                {
                    opts.AddReference(assemblyNode.Value);
                }
            }
            if (template.ParseResult.Metadata.TryGetValue("namespace", out data))
            {
                foreach (var namespaceNode in data)
                {
                    opts.AddUsing(namespaceNode.Value);
                }
            }

            MetaNode pagedefiniton = MetaDataFiller.FillAndGetPageDefinition(template.ParseResult.Metadata, opts);

            if (pagedefiniton.Attributes.Find(x => x.Name == "Inherits") != null)
            {
                string inherittype = ((TextChunk)((TextNode)pagedefiniton.Attributes.Find(x => x.Name == "Inherits").Value).Chunks[0]).Text;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var modelType = assembly.GetType(inherittype, false, true);
                    if (modelType != null)
                    {
                        opts.TemplateBaseType = ProxyExtractor.GetNonProxiedType(modelType);
                    }
                }
            }

            if (template.ParseResult.Metadata.TryGetValue("type", out data))
            {
                if ((opts.TemplateBaseType.IsGenericTypeDefinition) || ((BaseType!= null) && (origtype.IsGenericTypeDefinition)))
                {
                    string modeltypestring = data[0].Value;
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        var modelType = assembly.GetType(modeltypestring, false, true);
                        if (modelType != null)
                        {
                            if (opts.TemplateBaseType.IsGenericTypeDefinition)
                            {
                                opts.TemplateBaseType = opts.TemplateBaseType.MakeGenericType(ProxyExtractor.GetNonProxiedType(modelType));
                                break;
                            }
                            else if (origtype.IsGenericTypeDefinition)
                            {
                                opts.TemplateBaseType = origtype.MakeGenericType(ProxyExtractor.GetNonProxiedType(modelType));
                                break;
                            }
                        }
                    }
                }
            }
            if (opts.TemplateBaseType.IsGenericTypeDefinition)
            {
                opts.TemplateBaseType = opts.TemplateBaseType.MakeGenericType(typeof(object));
            }
            
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

            var templateCacheKey = template.Path;
            if (master != null) templateCacheKey = templateCacheKey + master.ContentFile.Path + opts.TemplateBaseType.FullName;

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