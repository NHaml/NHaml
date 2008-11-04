namespace NHaml.Rules
{
  public class EscapeMarkupRule : MarkupRule
  {
    public override char Signifier
    {
      get { return '\\'; }
    }

    public override BlockClosingAction Render(TemplateParser templateParser)
    {
      templateParser.TemplateClassBuilder.AppendOutput(templateParser.CurrentInputLine.Indent);
      templateParser.TemplateClassBuilder.AppendOutputLine(templateParser.CurrentInputLine.NormalizedText);

      return null;
    }
  }
}