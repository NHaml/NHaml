using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace NHaml.Compilers.FSharp
{
    internal class FSharpTemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public FSharpTemplateTypeBuilder(TemplateOptions options)
            : base(options)
        {
            ProviderOptions.Add( "CompilerVersion", "v2.0" );
        }


        protected override bool SupportsDebug()
        {
            return false;
        }

        protected override Type ExtractType(string typeName, Assembly assembly)
        {
            var fullTypeName = "TempNHamlNamespace." + typeName;
            return assembly.GetType(fullTypeName, true, true);
        }


        protected override void BuildSource( string source )
        {
            var sourceBuilder = new StringBuilder();

            sourceBuilder.AppendLine("#light ");
            sourceBuilder.AppendLine("namespace TempNHamlNamespace");
            foreach( var usingStatement in Options.Usings )
            {
                sourceBuilder.AppendLine( "open " + usingStatement);
            }

            sourceBuilder.AppendLine( source);

            Source = sourceBuilder.ToString();
        }
    }
}