namespace NHaml.Rules
{
  public class SilentEvalMarkupRule : MarkupRule
  {
    public const char SignifierChar = '-';

    public override char Signifier
    {
      get { return SignifierChar; }
    }

    public override bool MergeMultiline
    {
      get { return true; }
    }

    public override BlockClosingAction Render(TemplateParser templateParser)
    {
      return templateParser.TemplateEngine.TemplateCompiler.RenderSilentEval(templateParser);
    }
  }
}