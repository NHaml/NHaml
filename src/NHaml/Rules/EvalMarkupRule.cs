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

    public override BlockClosingAction Render(TemplateParser templateParser)
    {
      templateParser.TemplateClassBuilder
        .AppendOutput(templateParser.CurrentInputLine.Indent);
      templateParser.TemplateClassBuilder
        .AppendCodeLine(templateParser.CurrentInputLine.NormalizedText.Trim());

      return null;
    }
  }
}