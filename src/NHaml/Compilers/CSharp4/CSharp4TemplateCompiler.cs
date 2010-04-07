using NHaml.Compilers.CSharp3;

namespace NHaml.Compilers.CSharp4
{
    public class CSharp4TemplateCompiler : CSharp3TemplateCompiler
    {
      
        public override CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder(TemplateOptions options)
        {
            return new CSharp4TemplateTypeBuilder( options );
        }

    }
}