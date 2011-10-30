using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using NHaml.Compilers;
using NHaml.Compilers.CSharp3;
using NHaml.Rules;
using NHaml.TemplateResolution;
using NHaml.Utils;

namespace NHaml
{

    public delegate void Action<T1, T2>(T1 obj1, T2 obj2);
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
            ReferencedTypeHandles = new List<RuntimeTypeHandle>();
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
            AddRule(new SilentCommentMarkupRule());
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

        //TODO: prob should not make this public
        public Set<string> Usings { get; private set; }
        public Action<TemplateClassBuilder, Object> BeforeCompile { get; set; }
        List<RuntimeTypeHandle> ReferencedTypeHandles{ get; set; }

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
                {
                    throw new InvalidOperationException("TemplateBaseType must inherit from CompiledTemplate");
                }
                _templateBaseType = value;
             //   AddReferences(_templateBaseType);
                if (TemplateBaseTypeChanged != null)
                {
                    TemplateBaseTypeChanged(this, EventArgs.Empty);
                }
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
            MarkupRules.Sort((x, y) => y.Signifier.Length.CompareTo(x.Signifier.Length));
        }

        internal MarkupRule GetRule(InputLine inputLine)
        {
            Invariant.ArgumentNotNull(inputLine, "line");

            var start = inputLine.Text.TrimStart();

            return MarkupRules.FirstOrDefault(x => start.StartsWith(x.Signifier))
                ?? PlainTextMarkupRule.Instance;
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
            var typeHandle = type.TypeHandle;
            if (ReferencedTypeHandles.Contains(typeHandle))
            {
                return;
            }
            ReferencedTypeHandles.Add(typeHandle);
            try
            {
                AddReference(type.Assembly.Location);

                Usings.Add(type.Namespace);
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
            if (type.BaseType != null)
            {
                AddReferences(type.BaseType);
            }
            foreach (var @interface in type.GetInterfaces())
            {
                AddReferences(@interface);
            }
        }

        public void AddReference(string assemblyLocation)
        {
            Invariant.ArgumentNotEmpty(assemblyLocation, "assemblyLocation");
            //TODO: sort this with iron ruby
            if (assemblyLocation.ToLower().EndsWith("mscorlib.dll"))
            {
                return;
            }
            References.Add(assemblyLocation);
        }

        public void AddReference(Assembly assembly)
        {
            Invariant.ArgumentNotNull(assembly, "assembly");

            AddReference(assembly.Location);
        }
    }


}