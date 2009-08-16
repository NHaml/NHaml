using System.Diagnostics.CodeAnalysis;

namespace NHaml.Compilers.VisualBasic
{
    internal class VisualBasicTemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {

        [SuppressMessage( "Microsoft.Security", "CA2122" )]
        public VisualBasicTemplateTypeBuilder(TemplateOptions options)
            : base(options)
        {
            ProviderOptions.Add("CompilerVersion", "v3.5");
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}