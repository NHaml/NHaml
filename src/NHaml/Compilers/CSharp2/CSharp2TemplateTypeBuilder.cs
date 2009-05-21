using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp;

namespace NHaml.Compilers.CSharp2
{
    public class CSharp2TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CSharp2TemplateTypeBuilder( TemplateEngine templateEngine ) : base(templateEngine)
        {
            ProviderOptions.Add( "CompilerVersion", "v2.0" );
        }

        protected override CodeDomProvider GetCodeProvider()
        {
            return new CSharpCodeProvider(ProviderOptions);
        }

    }
}