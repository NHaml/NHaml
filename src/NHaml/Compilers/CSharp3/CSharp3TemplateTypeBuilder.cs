namespace NHaml.Compilers.CSharp3
{
    internal sealed class CSharp3TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {
        public CSharp3TemplateTypeBuilder(TemplateOptions options)
            : base( options )
        {
            ProviderOptions.Add("CompilerVersion", "v3.5");
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}