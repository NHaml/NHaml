using NHaml;
namespace NHaml4.Compilers.CSharp3
{
    internal class CSharp3TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {
        public CSharp3TemplateTypeBuilder()
            : base()
        {
            ProviderOptions.Add("CompilerVersion", "v3.5");
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}