using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualBasic;

namespace NHaml.Compilers.VisualBasic
{
    internal class VisualBasicTemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public VisualBasicTemplateTypeBuilder( TemplateEngine templateEngine ) : base(templateEngine)
        {
            ProviderOptions.Add("CompilerVersion", "v3.5");
        }


        protected override CodeDomProvider GetCodeProvider()
        {
            return new VBCodeProvider(ProviderOptions);
        }

       
    }
}