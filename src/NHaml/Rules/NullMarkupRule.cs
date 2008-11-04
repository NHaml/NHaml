using System.Diagnostics.CodeAnalysis;

namespace NHaml.Rules
{
  public class NullMarkupRule : MarkupRule
  {
    [SuppressMessage("Microsoft.Security", "CA2104")]
    public static readonly NullMarkupRule Instance = new NullMarkupRule();

    private NullMarkupRule()
    {
    }

    public override char Signifier
    {
      get { return new char(); }
    }

    public override BlockClosingAction Render(TemplateParser templateParser)
    {
      var text = templateParser.CurrentInputLine.Text;

      if ((templateParser.CurrentNode.Previous != null)
        && templateParser.CurrentNode.Previous.Value.IsMultiline)
      {
        text = text.TrimStart();
      }

      templateParser.TemplateClassBuilder.AppendOutput(text,
        !templateParser.CurrentInputLine.IsMultiline);

      return null;
    }
  }
}