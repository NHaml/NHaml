using System.Diagnostics.CodeAnalysis;
using Microsoft.CSharp;

namespace System.Web.NHaml.Compilers
{
    public class CSharp2TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public CSharp2TemplateTypeBuilder()
            : base(new CSharpCodeProvider())
        {
            ProviderOptions.Add( "CompilerVersion", "v2.0" );
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}

