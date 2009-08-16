namespace NHaml.Compilers.CSharp4
{
    internal sealed class CSharp4TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {
        public CSharp4TemplateTypeBuilder(TemplateOptions options)
            : base( options)
        {
            ProviderOptions.Add("CompilerVersion", "v4");
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}