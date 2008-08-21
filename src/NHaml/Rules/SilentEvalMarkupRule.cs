using System.Text.RegularExpressions;

namespace NHaml.Rules
{
  public class SilentEvalMarkupRule : MarkupRule
  {
    private static readonly Regex _lambdaRegex = new Regex(
      @"^(.+)(\(.*\))\s*=>\s*$",
      RegexOptions.Compiled | RegexOptions.Singleline);

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
      var code = compilationContext.CurrentInputLine.NormalizedText;

      var lambdaMatch = _lambdaRegex.Match(code);

      if (!lambdaMatch.Success)
      {
        var isBlock = (compilationContext.NextInputLine.IndentSize > compilationContext.CurrentInputLine.IndentSize);

        compilationContext.TemplateClassBuilder.AppendSilentCode(code, !isBlock);

        if (isBlock)
        {
          compilationContext.TemplateClassBuilder.BeginCodeBlock();

          return () => compilationContext.TemplateClassBuilder.EndCodeBlock("}");
        }

        return null;
      }

      var depth = compilationContext.CurrentInputLine.IndentSize;

      code = compilationContext.TemplateCompiler.LambdaRenderer.Render(code, lambdaMatch);

      compilationContext.TemplateClassBuilder.AppendSilentCode(code, depth);

      return () => compilationContext.TemplateClassBuilder.AppendSilentCode("});", depth);
    }
  }
}