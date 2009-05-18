using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp;

namespace NHaml.Compilers.CSharp2
{
    internal class CSharp2TemplateTypeBuilder
    {
        private readonly CompilerParameters _compilerParameters
          = new CompilerParameters();

        private readonly TemplateEngine _templateEngine;

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CSharp2TemplateTypeBuilder( TemplateEngine templateEngine )
        {
            ProviderOptions = new Dictionary<string, string>();
            _templateEngine = templateEngine;
            _templateEngine.AddReference(GetType().Assembly);

            ProviderOptions.Add( "CompilerVersion", "v2.0" );

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

            var codeProvider = new CSharpCodeProvider(ProviderOptions);

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

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        private void AddReferences()
        {
            _compilerParameters.ReferencedAssemblies.Clear();

            foreach( var assembly in _templateEngine.References )
            {
                _compilerParameters.ReferencedAssemblies.Add( assembly );
            }
        }

        private void BuildSource( string source )
        {
            Source = source;
        }
    }
}