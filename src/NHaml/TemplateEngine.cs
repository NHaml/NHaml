using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Configuration;
using NHaml.Rules;
using NHaml.TemplateResolution;
using NHaml.Utils;

namespace NHaml
{
    public sealed class TemplateEngine
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
            TemplateContentProvider = new FileTemplateContentProvider();
            Options.TemplateBaseTypeChanged += ( s, e ) => ClearCompiledTemplatesCache();
            Options.TemplateCompilerChanged += ( s, e ) => ClearCompiledTemplatesCache();
        }

        public TemplateOptions Options { get; private set; }

        public ITemplateContentProvider TemplateContentProvider { get; set; }

        private void ClearCompiledTemplatesCache()
        {
            lock( _compiledTemplateCache )
            {
                _compiledTemplateCache.Clear();
            }
        }

        internal MarkupRule GetRule( InputLine inputLine )
        {
            Invariant.ArgumentNotNull( inputLine, "line" );

            var start = inputLine.Text.TrimStart();
            foreach (var keyValuePair in Options.MarkupRules)
            {
                if (start.StartsWith(keyValuePair.Signifier))
                {
                    return keyValuePair;
                }
            }
            return PlainTextMarkupRule.Instance;
        }

        public CompiledTemplate Compile( string templatePath )
        {
            return Compile( templatePath, (string)null, Options.TemplateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, Type templateBaseType )
        {
            return Compile( templatePath, (string)null, templateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, string layoutTemplatePath )
        {
            return Compile( templatePath, layoutTemplatePath, Options.TemplateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, string layoutTemplatePath, Type templateBaseType )
        {
            if( string.IsNullOrEmpty( layoutTemplatePath ) )
                return Compile( templatePath, new List<string>(), templateBaseType );

            return Compile( templatePath, new List<string> {layoutTemplatePath}, templateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, List<string> layoutTemplatePaths )
        {
            return Compile( templatePath, layoutTemplatePaths, Options.TemplateBaseType );
        }

        public CompiledTemplate Compile(string  templatePath, IList<string> layoutTemplatePaths, Type templateBaseType)
        {
            var list = new List<IViewSource>();
            foreach (var layoutTemplatePath in layoutTemplatePaths)
            {
                list.Add(TemplateContentProvider.GetViewSource(layoutTemplatePath));
            }

            return Compile(TemplateContentProvider.GetViewSource(templatePath), list, templateBaseType);
        }


        public CompiledTemplate Compile( IViewSource templatePath, IList<IViewSource> layoutTemplatePaths, Type templateBaseType )
        {
            Invariant.ArgumentNotNull( templatePath, "templatePath" );
            Invariant.ArgumentNotNull( templateBaseType, "templateBaseType" );

            templateBaseType = GetNonProxiedBaseType(templateBaseType);
            var templateCacheKey = new StringBuilder( templatePath.Path );

            foreach( var layoutTemplatePath in layoutTemplatePaths )
            {
                templateCacheKey.AppendFormat( "{0}, ", layoutTemplatePath.Path );
            }

            CompiledTemplate compiledTemplate;

            lock( _compiledTemplateCache )
            {
                var key = templateCacheKey.ToString();
                if( !_compiledTemplateCache.TryGetValue( key, out compiledTemplate ) )
                {
                    compiledTemplate = new CompiledTemplate( this, templatePath, layoutTemplatePaths, templateBaseType );

                    _compiledTemplateCache.Add( key, compiledTemplate );
                }
            }

            if( Options.AutoRecompile )
            {
                compiledTemplate.Recompile();
            }

            return compiledTemplate;
        }

        /// <summary>
        /// Do a best guess on getting a non dynamic <see cref="Type"/>.
        /// </summary>
        /// <remarks>
        /// This is necessary for libraries like nhibernate that use proxied types.
        /// </remarks>
        private static Type GetNonProxiedBaseType(Type type)
        {
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                var genericArguments = new List<Type>();
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    genericArguments.Add(GetNonProxiedBaseType(genericArgument));
                }
                type = GetNonProxiedBaseType(type.GetGenericTypeDefinition());
                return type.MakeGenericType(genericArguments.ToArray());
            }

            if (IsDynamic(type))
            {
                var baseType = type.BaseType;
                if (baseType == typeof(object))
                {
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length > 1)
                    {
                        return GetNonProxiedBaseType(interfaces[0]);
                    }
                    else
                    {
                        throw new Exception(string.Format("Could not create non dynamic type for '{0}'.", type.FullName));
                    }
                }
                else
                {
                    return GetNonProxiedBaseType(baseType);
                }
            }
            return type;
        }

        //HACK: must be a better way of working this out
        private static bool IsDynamic(Type type)
        {
            try
            {
                var location = type.Assembly.Location;
                return false;
            }
            catch (Exception exception)
            {
                return true;
            }
        }
    }
}