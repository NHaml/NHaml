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

      compilationContext.TemplateClassBuilder.AppendSilentCode(compilationContext.CurrentInputLine.NormalizedText, !isBlock);

      if (isBlock)
      {
        compilationContext.TemplateClassBuilder.BeginCodeBlock();

        return () => compilationContext.TemplateClassBuilder.EndCodeBlock();
      }

      return null;
    }
  }
}