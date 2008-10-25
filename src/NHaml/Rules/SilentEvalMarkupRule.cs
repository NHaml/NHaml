namespace NHaml.Rules
{
  public class SilentEvalMarkupRule : MarkupRule
  {
    public override char Signifier
    {
      get { return '-'; }
    }

    public override bool MergeMultiline
    {
      get { return true; }
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      return compilationContext.SilentEvalRenderer.Render(compilationContext);
    }
  }
}