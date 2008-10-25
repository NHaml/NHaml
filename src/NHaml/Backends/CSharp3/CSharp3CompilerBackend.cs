using NHaml.BackEnds.CSharp2;

namespace NHaml.BackEnds.CSharp3
{
  public class CSharp3CompilerBackEnd : CSharp2CompilerBackEnd
  {
    public CSharp3CompilerBackEnd()
    {
      AttributeRenderer = new CSharp3AttributeRenderer();
      SilentEvalRenderer = new CSharp2SilentEvalRenderer(new CSharp3LambdaRenderer());
    }

    protected override CSharp2TemplateTypeBuilder CreateTemplateTypeBuilder(CompilationContext compilationContext)
    {
      return new CSharp3TemplateTypeBuilder(compilationContext.TemplateCompiler);
    }
  }
}