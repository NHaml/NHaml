using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using NHaml.Compilers;
using NHaml.Compilers.CSharp3;
using NHaml.Properties;
using NHaml.Rules;
using NHaml.Utils;

namespace NHaml
{
    public class TemplateOptions
    {
        private readonly IDictionary<string, IMarkupExtension> _markupExtensions;
        private int _indentSize;

        private Type _templateBaseType;
        private ITemplateCompiler _templateCompiler;
        private bool _useTabs;

        public TemplateOptions()
        {
            Usings = new Set<string>( new[] {"System", "System.IO", "System.Web", "NHaml", "NHaml.Utils", "System.Collections.Generic"} );
            References = new Set<string>( new[]
            {
                typeof(TemplateEngine).Assembly.Location, // NHaml
                typeof(INotifyPropertyChanged).Assembly.Location, // System
                typeof(HttpUtility).Assembly.Location // System.Web
            } );
            AutoClosingTags = new Set<string>( new[] {"META", "IMG", "LINK", "BR", "HR", "INPUT"} );

            MarkupRules = new MarkupRule[128];
            _indentSize = 2;
            _templateBaseType = typeof(Template);
            _templateCompiler = new CSharp3TemplateCompiler();
            _markupExtensions = new Dictionary<string, IMarkupExtension>();

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
            AddRule( new NamedExtensionRule() );
        }

        public Set<string> AutoClosingTags { get; private set; }

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

        public Set<string> Usings { get; private set; }

        public Set<string>  References { get; private set; }

        public MarkupRule[] MarkupRules { get; private set; }

        public ITemplateCompiler TemplateCompiler
        {
            get { return _templateCompiler; }
            set
            {
                Invariant.ArgumentNotNull( value, "value" );

                _templateCompiler = value;

                if( TemplateCompilerChanged != null )
                    TemplateCompilerChanged( this, EventArgs.Empty );
            }
        }

        public Type TemplateBaseType
        {
            get { return _templateBaseType; }
            set
            {
                Invariant.ArgumentNotNull( value, "value" );

                if( !typeof(Template).IsAssignableFrom( value ) )
                    throw new InvalidOperationException( Resources.InvalidTemplateBaseType );

                _templateBaseType = value;

                AddReferences( _templateBaseType );

                Usings.Add( _templateBaseType.Namespace );

                if( TemplateBaseTypeChanged != null )
                    TemplateBaseTypeChanged( this, EventArgs.Empty );
            }
        }

        public void AddExtension( IMarkupExtension markupExtension )
        {
            Invariant.ArgumentNotNull( markupExtension, "markupExtension" );

            _markupExtensions.Add( markupExtension.Name, markupExtension );
        }

        public IMarkupExtension GetExtension( string name )
        {
            Invariant.ArgumentNotNull( name, "name" );

            return _markupExtensions[name];
        }

        public event EventHandler TemplateCompilerChanged;
        public event EventHandler TemplateBaseTypeChanged;

        public void AddRule( MarkupRule markupRule )
        {
            Invariant.ArgumentNotNull( markupRule, "markupRule" );

            MarkupRules[markupRule.Signifier] = markupRule;
        }

        public bool IsAutoClosingTag( string tag )
        {
            Invariant.ArgumentNotEmpty( tag, "tag" );

            return AutoClosingTags.Contains( tag.ToUpperInvariant() );
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
                return;

            foreach( var t in type.GetGenericArguments() )
                AddReferences( t );
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
    }
}