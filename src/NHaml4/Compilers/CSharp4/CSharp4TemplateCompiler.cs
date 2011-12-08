using NHaml4.Compilers.CSharp3;
using NHaml;

namespace NHaml4.Compilers.CSharp4
{
    public class CSharp4TemplateCompiler : CSharp3TemplateCompiler
    {
      
        public override CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder(TemplateOptions options)
        {
            return new CSharp4TemplateTypeBuilder( options );
        }

    }
}