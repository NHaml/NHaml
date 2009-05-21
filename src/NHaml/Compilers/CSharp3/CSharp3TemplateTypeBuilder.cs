using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace NHaml.Compilers.CSharp3
{
    internal sealed class CSharp3TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {
        public CSharp3TemplateTypeBuilder( TemplateEngine templateEngine )
            : base( templateEngine )
        {
            ProviderOptions.Add("CompilerVersion", "v3.5");
        }

        protected override CodeDomProvider GetCodeProvider()
        {
            return new CSharpCodeProvider(ProviderOptions);
        }
    }
}