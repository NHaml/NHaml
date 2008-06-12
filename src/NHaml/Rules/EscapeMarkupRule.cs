namespace NHaml.Rules
{
  public class EscapeMarkupRule : MarkupRule
  {
    public override char Signifier
    {
      get { return '\\'; }
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      compilationContext.ViewBuilder.AppendOutputLine(
        compilationContext.CurrentInputLine.Indent
          + compilationContext.CurrentInputLine.NormalizedText);

      return null;
    }
  }
}