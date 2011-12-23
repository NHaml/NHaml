using System.Diagnostics.CodeAnalysis;
using NHaml;
using Microsoft.CSharp;
using NHaml4.Compilers.Abstract;

namespace NHaml4.Compilers.CSharp2
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

