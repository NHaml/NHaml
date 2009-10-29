using System.Diagnostics.CodeAnalysis;
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