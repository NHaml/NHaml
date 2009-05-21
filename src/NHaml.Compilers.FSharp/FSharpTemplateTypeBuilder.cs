using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.FSharp.Compiler.CodeDom;

namespace NHaml.Compilers.FSharp
{
    internal class FSharpTemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public FSharpTemplateTypeBuilder( TemplateEngine templateEngine ) : base(templateEngine)
        {
            ProviderOptions.Add( "CompilerVersion", "v2.0" );
        }


        protected override Type ExtractType(string typeName)
        {
            var assembly = CompilerResults.CompiledAssembly;
            var fullTypeName = "TempNHamlNamespace." + typeName;
            return assembly.GetType(fullTypeName, true, true);

        }
        protected override CodeDomProvider GetCodeProvider()
        {
            return new FSharpCodeProvider();
        }


        protected override void BuildSource( string source )
        {
            var sourceBuilder = new StringBuilder();

            sourceBuilder.AppendLine("#light ");
            sourceBuilder.AppendLine("namespace TempNHamlNamespace");
            foreach( var usingStatement in TemplateEngine.Usings )
            {
                sourceBuilder.AppendLine( "open " + usingStatement);
            }

            sourceBuilder.AppendLine( source);

            Source = sourceBuilder.ToString();
        }
    }
}