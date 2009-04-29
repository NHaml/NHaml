using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
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
          = new[] { "System", "System.IO", "System.Web", "NHaml", "NHaml.Utils" };

        private readonly StringSet _autoClosingTags =
          new StringSet( DefaultAutoClosingTags );

        private readonly MarkupRule[] _markupRules = new MarkupRule[128];

        private readonly StringSet _references = new StringSet( DefaultReferences );
        private readonly StringSet _usings = new StringSet( DefaultUsings );

        private readonly Dictionary<string, CompiledTemplate> _compiledTemplateCache
          = new Dictionary<string, CompiledTemplate>();

        private ITemplateCompiler _templateCompiler = new CSharp3TemplateCompiler();
        private Type _templateBaseType = typeof( Template );

        private bool _useTabs;
        private int _indentSize = 2;

        public TemplateEngine()
        {
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

                _usings.Add( _templateBaseType.Namespace );

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

        public IEnumerable<string> Usings
        {
            get { return _usings; }
        }

        public IEnumerable<string> References
        {
            get { return _references; }
        }

        internal void AddRule( MarkupRule markupRule )
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

            _usings.Add( @namespace );
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

            _references.Add( assemblyLocation );
        }

        public void AddReference( Assembly assembly )
        {
            Invariant.ArgumentNotNull( assembly, "assembly" );

            _references.Add( assembly.Location );
        }

        public CompiledTemplate Compile( string templatePath )
        {
            return Compile( templatePath, null, TemplateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, Type templateBaseType )
        {
            return Compile( templatePath, null, templateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, string layoutTemplatePath )
        {
            return Compile( templatePath, layoutTemplatePath, TemplateBaseType );
        }

        public CompiledTemplate Compile( string templatePath, string layoutTemplatePath, Type templateBaseType )
        {
            Invariant.ArgumentNotEmpty( templatePath, "templatePath" );
            Invariant.FileExists( templatePath );
            Invariant.ArgumentNotNull( templateBaseType, "templateBaseType" );

            if( !string.IsNullOrEmpty( layoutTemplatePath ) )
            {
                Invariant.FileExists( layoutTemplatePath );
            }

            var templateCacheKey = templatePath + layoutTemplatePath;
            CompiledTemplate compiledTemplate;

            lock( _compiledTemplateCache )
            {
                if( !_compiledTemplateCache.TryGetValue( templateCacheKey, out compiledTemplate ) )
                {
                    compiledTemplate = new CompiledTemplate( this, templatePath, layoutTemplatePath, templateBaseType );

                    _compiledTemplateCache.Add( templateCacheKey, compiledTemplate );
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