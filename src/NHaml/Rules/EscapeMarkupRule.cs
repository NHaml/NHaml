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
      compilationContext.TemplateClassBuilder.AppendOutput(compilationContext.CurrentInputLine.Indent);
      compilationContext.TemplateClassBuilder.AppendOutputLine(compilationContext.CurrentInputLine.NormalizedText);

      return null;
    }
  }
}