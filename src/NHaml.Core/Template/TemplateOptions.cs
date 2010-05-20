using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using NHaml.Core.Compilers;
using NHaml.Core.TemplateResolution;
using NHaml.Core.Utils;

namespace NHaml.Core.Template
{
    public delegate void Action<T1, T2>(T1 obj1, T2 obj2);
    public class TemplateOptions
    {
        private Type _templateBaseType;
        private IClassBuilder _templateCompiler;

        public TemplateOptions()
        {
            Usings = new Set<string>(new[] { "System", "System.IO", "System.Web", "NHaml.Core", "NHaml.Core.Utils", "System.Collections.Generic" });
            References = new Set<string>(new[]
            {
                typeof(TemplateEngine).Assembly.Location, // NHaml
                typeof(INotifyPropertyChanged).Assembly.Location, // System
                typeof(HttpUtility).Assembly.Location // System.Web
            });
            AutoClosingTags = new Set<string>(new[] { "META", "IMG", "LINK", "BR", "HR", "INPUT" });
            ReferencedTypeHandles = new List<RuntimeTypeHandle>();
            _templateBaseType = typeof(Template);
            TemplateContentProvider = new FileTemplateContentProvider();
        }

        public Set<string> AutoClosingTags { get; private set; }

        public bool AutoRecompile { get; set; }
        public bool EncodeHtml { get; set; }
        public bool OutputDebugFiles { get; set; }

        public ITemplateContentProvider TemplateContentProvider { get; set; }

        //TODO: prob should not make this public
        public Set<string> Usings { get; private set; }
        public Action<IClassBuilder, Object> BeforeCompile { get; set; }
        List<RuntimeTypeHandle> ReferencedTypeHandles{ get; set; }

        public Set<string> References { get; private set; }

        public IClassBuilder TemplateCompiler
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