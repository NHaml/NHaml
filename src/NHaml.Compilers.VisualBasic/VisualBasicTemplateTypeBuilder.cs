using System.Diagnostics.CodeAnalysis;

namespace NHaml.Compilers.VisualBasic
{
    internal class VisualBasicTemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public VisualBasicTemplateTypeBuilder( TemplateEngine templateEngine ) : base(templateEngine)
        {
            ProviderOptions.Add("CompilerVersion", "v3.5");
        }



       
    }
}