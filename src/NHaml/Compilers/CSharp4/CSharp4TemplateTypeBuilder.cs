namespace NHaml.Compilers.CSharp4
{
    internal sealed class CSharp4TemplateTypeBuilder : CodeDomTemplateTypeBuilder
    {
        public CSharp4TemplateTypeBuilder( TemplateEngine templateEngine )
            : base( templateEngine )
        {
            ProviderOptions.Add("CompilerVersion", "v4");
        }


        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}