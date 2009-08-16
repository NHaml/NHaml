using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using NHaml.Compilers;
using NHaml.Compilers.CSharp3;
using NHaml.Rules;
using NHaml.TemplateResolution;
using NHaml.Utils;

namespace NHaml
{
    public class TemplateOptions
    {
        private int _indentSize;

        private Type _templateBaseType;
        private ITemplateCompiler _templateCompiler;
        private bool _useTabs;

        public TemplateOptions()
        {
            Usings = new Set<string>(new[] { "System", "System.IO", "System.Web", "NHaml", "NHaml.Utils", "System.Collections.Generic" });
            References = new Set<string>(new[]
            {
                typeof(TemplateEngine).Assembly.Location, // NHaml
                typeof(INotifyPropertyChanged).Assembly.Location, // System
                typeof(HttpUtility).Assembly.Location // System.Web
            });
            AutoClosingTags = new Set<string>(new[] { "META", "IMG", "LINK", "BR", "HR", "INPUT" });

            MarkupRules = new List<MarkupRule>();
            _indentSize = 2;
            _templateBaseType = typeof(Template);
            _templateCompiler = new CSharp3TemplateCompiler();
            TemplateContentProvider = new FileTemplateContentProvider();

            AddRule(new EofMarkupRule());
            AddRule(new MetaMarkupRule());
            AddRule(new DocTypeMarkupRule());
            AddRule(new TagMarkupRule());
            AddRule(new ClassMarkupRule());
            AddRule(new IdMarkupRule());
            AddRule(new EvalMarkupRule());
            AddRule(new EncodedEvalMarkupRule());
            AddRule(new SilentEvalMarkupRule());
            AddRule(new PreambleMarkupRule());
            AddRule(new CommentMarkupRule());
            AddRule(new EscapeMarkupRule());
            AddRule(new PartialMarkupRule());
            AddRule(new NotEncodedEvalMarkupRule());
        }

        public Set<string> AutoClosingTags { get; private set; }

        public bool AutoRecompile { get; set; }
        public bool EncodeHtml { get; set; }
        public bool OutputDebugFiles { get; set; }

        public bool UseTabs
        {
            get { return _useTabs; }
            set
            {
                _useTabs = value;

                IndentSize = _indentSize;
            }
        }
        public ITemplateContentProvider TemplateContentProvider { get; set; }

        public int IndentSize
        {
            get { return _indentSize; }
            set { _indentSize = UseTabs ? 1 : Math.Max(2, value); }
        }

        public Set<string> Usings { get; private set; }

        public Set<string> References { get; private set; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// Use <see cref="AddRule"/> to add new rules
        /// </remarks>
        public List<MarkupRule> MarkupRules { get; private set; }

        public ITemplateCompiler TemplateCompiler
        {
            get { return _templateCompiler; }
            set
            {
                Invariant.ArgumentNotNull(value, "value");

                _templateCompiler = value;

                if (TemplateCompilerChanged != null)
                    TemplateCompilerChanged(this, EventArgs.Empty);
            }
        }

        public Type TemplateBaseType
        {
            get { return _templateBaseType; }
            set
            {
                Invariant.ArgumentNotNull(value, "value");

                if (!typeof(Template).IsAssignableFrom(value))
                    throw new InvalidOperationException("TemplateBaseType must inherit from CompiledTemplate");

                _templateBaseType = value;

                AddReferences(_templateBaseType);

                Usings.Add(_templateBaseType.Namespace);

                if (TemplateBaseTypeChanged != null)
                    TemplateBaseTypeChanged(this, EventArgs.Empty);
            }
        }


        public event EventHandler TemplateCompilerChanged;
        public event EventHandler TemplateBaseTypeChanged;

        public void AddRule(MarkupRule markupRule)
        {
            Invariant.ArgumentNotNull(markupRule, "markupRule");
            if (MarkupRules.Find(x => x.Signifier == markupRule.Signifier) != null)
            {
                throw new ArgumentException(string.Format("A MarkupRule with the signifier '{0}' has already been added.", markupRule.Signifier));
            }
            MarkupRules.Add(markupRule);
            MarkupRules.Sort((x, y) => x.Signifier.Length.CompareTo(y.Signifier.Length));
        }

        internal MarkupRule GetRule(InputLine inputLine)
        {
            Invariant.ArgumentNotNull(inputLine, "line");

            var start = inputLine.Text.TrimStart();
            foreach (var keyValuePair in MarkupRules)
            {
                if (start.StartsWith(keyValuePair.Signifier))
                {
                    return keyValuePair;
                }
            }
            return PlainTextMarkupRule.Instance;
        }

        public bool IsAutoClosingTag(string tag)
        {
            Invariant.ArgumentNotEmpty(tag, "tag");

            return AutoClosingTags.Contains(tag.ToUpperInvariant());
        }

        public void AddUsing(string @namespace)
        {
            Invariant.ArgumentNotEmpty(@namespace, "namespace");

            Usings.Add(@namespace);
        }

        public void AddReferences(Type type)
        {
            try
            {

                AddReference(type.Assembly.Location);
            }
            catch (NotSupportedException)
            {
                //swallow for dynamic types
            }

            if (type.IsGenericType)
            {
                foreach (var genericArgument in type.GetGenericArguments())
                {
                    AddReferences(genericArgument);
                }
            }
        }

        public void AddReference(string assemblyLocation)
        {
            Invariant.ArgumentNotEmpty(assemblyLocation, "assemblyLocation");
            //TODO: sort this with iron ruby
#if NET4
            if (assemblyLocation.ToLower().EndsWith("mscorlib.dll"))
            {
                return;
            }
#endif
            References.Add(assemblyLocation);
        }

        public void AddReference(Assembly assembly)
        {
            Invariant.ArgumentNotNull(assembly, "assembly");

            AddReference(assembly.Location);
        }
    }


}