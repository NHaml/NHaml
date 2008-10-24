namespace NHaml.Rules
{
  public class PreambleMarkupRule : SilentEvalMarkupRule
  {
    public override char Signifier
    {
      get { return '^'; }
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      compilationContext.TemplateClassBuilder.AppendPreambleCode(compilationContext.CurrentInputLine.NormalizedText);

      return null;
    }
  }
}