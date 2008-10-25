using NHaml.BackEnds.CSharp2;

namespace NHaml.BackEnds.CSharp3
{
  public class CSharp3TemplateTypeBuilder : CSharp2TemplateTypeBuilder
  {
    public CSharp3TemplateTypeBuilder(TemplateCompiler templateCompiler)
      : base(templateCompiler)
    {
      ProviderOptions["CompilerVersion"] = "v3.5";
    }
  }
}