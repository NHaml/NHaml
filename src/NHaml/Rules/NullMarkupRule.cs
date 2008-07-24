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

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      var text = compilationContext.CurrentInputLine.Text;

      if ((compilationContext.CurrentNode.Previous != null)
        && compilationContext.CurrentNode.Previous.Value.IsMultiline)
      {
        text = text.TrimStart();
      }

      compilationContext.ViewBuilder.AppendOutput(text,
        !compilationContext.CurrentInputLine.IsMultiline);

      return null;
    }
  }
}