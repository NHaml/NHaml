using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace NHaml.BackEnds.Boo
{
  [SuppressMessage("Microsoft.Naming", "CA1722")]
  public sealed class BooSilentEvalRenderer : ISilentEvalRenderer
  {
    public static readonly Regex LambdaRegex = new Regex(
      @"^(.+)(def\(.*\))\s*$",
      RegexOptions.Compiled | RegexOptions.Singleline);

    #region ISilentEvalRenderer Members

    public BlockClosingAction Render(CompilationContext compilationContext)
    {
      var code = compilationContext.CurrentInputLine.NormalizedText;

      var lambdaMatch = LambdaRegex.Match(code);

      if (!lambdaMatch.Success)
      {
        compilationContext.TemplateClassBuilder.AppendSilentCode(code, !compilationContext.IsBlock);

        if (compilationContext.IsBlock)
        {
          compilationContext.TemplateClassBuilder.BeginCodeBlock();

          return () => compilationContext.TemplateClassBuilder.EndCodeBlock();
        }

        return null;
      }

      var booTemplateClassBuilder = (BooTemplateClassBuilder)compilationContext.TemplateClassBuilder;
      var depth = compilationContext.CurrentInputLine.IndentSize;

      booTemplateClassBuilder.AppendChangeOutputDepth(depth, true);
      booTemplateClassBuilder.AppendSilentCode(code, false);
      booTemplateClassBuilder.BeginCodeBlock();

      return () =>
        {
          booTemplateClassBuilder.AppendChangeOutputDepth(depth, false);
          booTemplateClassBuilder.EndCodeBlock();
        };
    }

    #endregion
  }
}