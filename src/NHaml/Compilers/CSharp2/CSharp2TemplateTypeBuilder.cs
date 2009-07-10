using System.Diagnostics.CodeAnalysis;

namespace NHaml.Compilers.CSharp2
{
    public class CSharp2TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CSharp2TemplateTypeBuilder( TemplateEngine templateEngine ) : base(templateEngine)
        {
            ProviderOptions.Add( "CompilerVersion", "v2.0" );
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}

