using NHaml;
namespace NHaml4.Compilers.CSharp4
{
    internal class CSharp4TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {
        public CSharp4TemplateTypeBuilder(TemplateOptions options)
            : base( options)
        {
            ProviderOptions.Add("CompilerVersion", "v4.0");
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}