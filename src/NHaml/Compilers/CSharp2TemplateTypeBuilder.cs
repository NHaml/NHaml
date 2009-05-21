using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NHaml.Compilers
{
    public abstract class CodeDomTemplateTypeBuilder : ITemplateTypeBuilder
    {
        private readonly CompilerParameters _compilerParameters
            = new CompilerParameters();

        private readonly TemplateEngine _templateEngine;

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CodeDomTemplateTypeBuilder( TemplateEngine templateEngine )
        {
            ProviderOptions = new Dictionary<string, string>();
            _templateEngine = templateEngine;
            _templateEngine.AddReference(GetType().Assembly);
            _compilerParameters.GenerateInMemory = true;
            _compilerParameters.IncludeDebugInformation = false;
        }

        public string Source { get; private set; }

        public CompilerResults CompilerResults { get; private set; }

        protected Dictionary<string, string> ProviderOptions { get; private set; }

        [SuppressMessage("Microsoft.Security", "CA2122")]
        [SuppressMessage("Microsoft.Portability", "CA1903")]
        public Type Build(string source, string typeName)
        {
            BuildSource(source);

            Trace.WriteLine(Source);

            AddReferences();

            var codeProvider = GetCodeProvider();

            CompilerResults = codeProvider
                .CompileAssemblyFromSource(_compilerParameters, Source);
            foreach (CompilerError result in CompilerResults.Errors)
            {
                if (!result.IsWarning)
                {
                    return null;
                }
            }

            return CompilerResults.CompiledAssembly.GetType(typeName);

        }

        protected abstract CodeDomProvider GetCodeProvider();

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        private void AddReferences()
        {
            _compilerParameters.ReferencedAssemblies.Clear();

            foreach( var assembly in _templateEngine.References )
            {
                _compilerParameters.ReferencedAssemblies.Add( assembly );
            }
        }

        protected virtual void BuildSource( string source )
        {
            Source = source;
        }
    }
}