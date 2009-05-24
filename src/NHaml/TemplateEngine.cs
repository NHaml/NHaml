using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Web;

using NHaml.Compilers;
using NHaml.Compilers.CSharp3;
using NHaml.Configuration;
using NHaml.Properties;
using NHaml.Rules;
using NHaml.Utils;

namespace NHaml
{
    public sealed class TemplateEngine
    {
        private static readonly string[] DefaultAutoClosingTags
          = new[] { "META", "IMG", "LINK", "BR", "HR", "INPUT" };

        private static readonly string[] DefaultReferences
          = new[]
          {
            typeof(TemplateEngine).Assembly.Location,
            typeof(INotifyPropertyChanged).Assembly.Location,
            typeof(HttpUtility).Assembly.Location
          };

        private static readonly string[] DefaultUsings
          = new[] { "System", "System.IO", "System.Web", "NHaml", "NHaml.Utils","System.Collections.Generic" };

        private readonly StringSet _autoClosingTags =
          new StringSet( DefaultAutoClosingTags );

        private readonly MarkupRule[] _markupRules;

        private readonly Dictionary<string, CompiledTemplate> _compiledTemplateCache;

        private ITemplateCompiler _templateCompiler;
        private Type _templateBaseType;

        private bool _useTabs;
        private int _indentSize;

        public TemplateEngine()
        {
            _markupRules = new MarkupRule[128];
            _compiledTemplateCache = new Dictionary<string, CompiledTemplate>();
            Usings = new StringSet( DefaultUsings );
            References = new StringSet( DefaultReferences );
            _indentSize = 2;
            _templateBaseType = typeof( Template );
            _templateCompiler = new CSharp3TemplateCompiler();
            AddRule( new EofMarkupRule() );
            AddRule( new MetaMarkupRule() );
            AddRule( new DocTypeMarkupRule() );
            AddRule( new TagMarkupRule() );
            AddRule( new ClassMarkupRule() );
            AddRule( new IdMarkupRule() );
            AddRule( new EvalMarkupRule() );
            AddRule( new EncodedEvalMarkupRule() );
            AddRule( new SilentEvalMarkupRule() );
            AddRule( new PreambleMarkupRule() );
            AddRule( new CommentMarkupRule() );
            AddRule( new EscapeMarkupRule() );
            AddRule( new PartialMarkupRule() );

            Configure();
        }

        private void Configure()
        {
            var section = NHamlConfigurationSection.GetSection();

            if( section == null )
            {
                return;
            }

            if( section.AutoRecompile.HasValue )
            {
                AutoRecompile = section.AutoRecompile.Value;
            }

            if( section.UseTabs.HasValue )
            {
                UseTabs = section.UseTabs.Value;
            }

            if( section.EncodeHtml.HasValue )
            {
                EncodeHtml = section.EncodeHtml.Value;
            }

            if( !string.IsNullOrEmpty( section.TemplateCompiler ) )
            {
                TemplateCompiler = section.CreateTemplateCompiler();
            }

            foreach( var assemblyConfigurationElement in section.Assemblies )
            {
                AddReference( Assembly.Load( assemblyConfigurationElement.Name ).Location );
            }

            foreach( var namespaceConfigurationElement in section.Namespaces )
            {
                AddUsing( namespaceConfigurationElement.Name );
            }
        }

        public ITemplateCompiler TemplateCompiler
        {
            get { return _templateCompiler; }
            set
            {
                Invariant.ArgumentNotNull( value, "value" );

                _templateCompiler = value;

                ClearCompiledTemplatesCache();
            }
        }

        public bool AutoRecompile { get; set; }
        public bool EncodeHtml { get; set; }

        public bool UseTabs
        {
            get { return _useTabs; }
            set
            {
                _useTabs = value;

                IndentSize = _indentSize;
            }
        }

        public int IndentSize
        {
            get { return _indentSize; }
            set { _indentSize = UseTabs ? 1 : Math.Max( 2, value ); }
        }

        public Type TemplateBaseType
        {
            get { return _templateBaseType; }
            set
            {
                Invariant.ArgumentNotNull( value, "value" );

                if( !typeof( Template ).IsAssignableFrom( value ) )
                {
                    throw new InvalidOperationException( Resources.InvalidTemplateBaseType );
                }

                _templateBaseType = value;

                AddReferences( _templateBaseType );

                Usings.Add( _templateBaseType.Namespace );

                ClearCompiledTemplatesCache();
            }
        }

        private void ClearCompiledTemplatesCache()
        {
            lock( _compiledTemplateCache )
            {
                _compiledTemplateCache.Clear();
            }
        }

        public StringSet Usings { get; private set; }

        public StringSet References { get; private set; }

        public void AddRule( MarkupRule markupRule )
        {
            Invariant.ArgumentNotNull( markupRule, "markupRule" );

            _markupRules[markupRule.Signifier] = markupRule;
        }

        internal MarkupRule GetRule( InputLine inputLine )
        {
            Invariant.ArgumentNotNull( inputLine, "line" );

            if( inputLine.Signifier >= 128 )
            {
                return PlainTextMarkupRule.Instance;
            }

            return _markupRules[inputLine.Signifier] ?? PlainTextMarkupRule.Instance;
        }

        internal bool IsAutoClosingTag( string tag )
        {
            Invariant.ArgumentNotEmpty( tag, "tag" );

            return _autoClosingTags.Contains( tag.ToUpperInvariant() );
        }

        public void AddUsing( string @namespace )
        {
            Invariant.ArgumentNotEmpty( @namespace, "namespace" );

            Usings.Add( @namespace );
        }

        public void AddReferences( Type type )
        {
            AddReference( type.Assembly.Location );

            if( !type.IsGenericType )
            {
                return;
            }

            foreach( var t in type.GetGenericArguments() )
            {
                AddReferences( t );
            }
        }

        public void AddReference( string assemblyLocation )
        {
            Invariant.ArgumentNotEmpty( assemblyLocation, "assemblyLocation" );

            References.Add( assemblyLocation );
        }

        public void AddReference( Assembly assembly )
        {
            Invariant.ArgumentNotNull( assembly, "assembly" );

            References.Add( assembly.Location );
        }

        public CompiledTemplate Compile( string templatePath )
        {
            return Compile( templatePath, (string)null, TemplateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, Type templateBaseType )
        {
            return Compile( templatePath, (string)null, templateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, string layoutTemplatePath )
        {
            return Compile( templatePath, layoutTemplatePath, TemplateBaseType );
        }

        public CompiledTemplate Compile(string templatePath, string layoutTemplatePath, Type templateBaseType)
        {
            if (string.IsNullOrEmpty(layoutTemplatePath))
            {
                return Compile(templatePath, new List<string>(), templateBaseType);
            }
            else
            {
                return Compile(templatePath, new List<string> {layoutTemplatePath}, templateBaseType);
            }
        }

        public CompiledTemplate Compile(string templatePath, List<string> layoutTemplatePaths)
        {
            return Compile(templatePath, layoutTemplatePaths, TemplateBaseType);
        }

        public CompiledTemplate Compile( string templatePath, IList<string> layoutTemplatePaths, Type templateBaseType )
        {
            Invariant.ArgumentNotEmpty( templatePath, "templatePath" );
            Invariant.FileExists( templatePath );
            Invariant.ArgumentNotNull( templateBaseType, "templateBaseType" );

            var templateCacheKey = new StringBuilder(templatePath );
            
            foreach (var layoutTemplatePath in layoutTemplatePaths)
            {
                    Invariant.FileExists(layoutTemplatePath);
                templateCacheKey.AppendFormat("{0}, ", layoutTemplatePath);
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

            if( AutoRecompile )
            {
                compiledTemplate.Recompile();
            }

            return compiledTemplate;
        }
    }
}