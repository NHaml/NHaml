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
      var isBlock = compilationContext.NextInputLine.IndentSize > compilationContext.CurrentInputLine.IndentSize;

      compilationContext.ViewBuilder.AppendSilentCode(compilationContext.CurrentInputLine.NormalizedText, !isBlock);

      if (isBlock)
      {
        compilationContext.ViewBuilder.BeginCodeBlock();

        return () => compilationContext.ViewBuilder.EndCodeBlock();
      }

      return null;
    }
  }
}