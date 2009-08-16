using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Parser;

namespace NHaml.Compilers.Boo
{
    internal  class BooTemplateTypeBuilder
    {
        private readonly BooCompiler _booCompiler;

        private readonly TemplateOptions options;

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public BooTemplateTypeBuilder(TemplateOptions options)
        {
            _booCompiler = new BooCompiler();
            CompilerResults = new CompilerResults( new TempFileCollection() );
            this.options = options;

            _booCompiler.Parameters.GenerateInMemory = true;
            _booCompiler.Parameters.Debug = true;
            _booCompiler.Parameters.OutputType = CompilerOutputType.Library;
        }

        public string Source { get; private set; }

        public CompilerResults CompilerResults { get; private set; }

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        [SuppressMessage( "Microsoft.Portability", "CA1903" )]
        public Type Build( string source, string typeName )
        {
            BuildSource( source );
            AddReferences();

            _booCompiler.Parameters.Input.Clear();
            _booCompiler.Parameters.Input.Add(new StringInput(typeName, Source)); 
            _booCompiler.Parameters.References.Add(typeof(BooParser).Assembly);
            _booCompiler.Parameters.Pipeline = new CompileToMemory();
            var context = _booCompiler.Run();

            if( context.Errors.Count == 0 )
            {
                return context.GeneratedAssembly.GetType( typeName );
            }

            CompilerResults.Errors.Clear();
            foreach(var error in context.Errors )
            {
                CompilerResults.Errors.Add( new System.CodeDom.Compiler.CompilerError(
                  error.LexicalInfo.FileName ?? String.Empty,
                  error.LexicalInfo.Line,
                  error.LexicalInfo.Column,
                  error.Code,
                  error.Message ) );
            }

            return null;
        }

        [SuppressMessage( "Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods" )]
        private void AddReferences()
        {
            foreach( var assemblyFile in options.References )
            {
                var assembly = Assembly.LoadFrom( assemblyFile );

                _booCompiler.Parameters.References.Add( assembly );
            }
        }

        private void BuildSource( string source )
        {
            var sourceBuilder = new StringBuilder();

            foreach( var usingStatement in options.Usings )
            {
                sourceBuilder.AppendLine( "import " + usingStatement );
            }

            sourceBuilder.AppendLine( source );

            Source = sourceBuilder.ToString();
        }
    }
}