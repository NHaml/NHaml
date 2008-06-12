using System;

namespace NHaml.Rules
{
  public class EofMarkupRule : MarkupRule
  {
    public static readonly char SignifierChar = Convert.ToChar(26);

    public override char Signifier
    {
      get { return SignifierChar; }
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      return null;
    }
  }
}