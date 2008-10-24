namespace NHaml.Rules
{
  public class EvalMarkupRule : MarkupRule
  {
    public override char Signifier
    {
      get { return '='; }
    }

    public override bool MergeMultiline
    {
      get { return true; }
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      compilationContext.TemplateClassBuilder
        .AppendOutput(compilationContext.CurrentInputLine.Indent);
      compilationContext.TemplateClassBuilder
        .AppendCodeLine(compilationContext.CurrentInputLine.NormalizedText.Trim());

      return null;
    }
  }
}