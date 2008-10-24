using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace NHaml.Backends.CSharp2
{
  [SuppressMessage("Microsoft.Naming", "CA1722")]
  public class CSharp2SilentEvalRenderer : ISilentEvalRenderer
  {
    public static readonly Regex LambdaRegex = new Regex(
      @"^(.+)(\(.*\))\s*=>\s*$",
      RegexOptions.Compiled | RegexOptions.Singleline);

    public CSharp2SilentEvalRenderer(ILambdaRenderer iLambdaRenderer)
    {
      LambdaRenderer = iLambdaRenderer;
    }

    public ILambdaRenderer LambdaRenderer { get; set; }

    public BlockClosingAction Render(CompilationContext compilationContext)
    {
      var code = compilationContext.CurrentInputLine.NormalizedText;

      var lambdaMatch = LambdaRegex.Match(code);

      if (!lambdaMatch.Success)
      {
        compilationContext.TemplateClassBuilder
          .AppendSilentCode(code, !compilationContext.IsBlock);

        if (compilationContext.IsBlock)
        {
          compilationContext.TemplateClassBuilder.BeginCodeBlock();

          return () => compilationContext.TemplateClassBuilder.EndCodeBlock();
        }

        return null;
      }

      var depth = compilationContext.CurrentInputLine.IndentSize;

      code = LambdaRenderer.Render(code, lambdaMatch);

      compilationContext.TemplateClassBuilder.AppendChangeOutputDepth(depth);
      compilationContext.TemplateClassBuilder.AppendSilentCode(code, true);

      return () =>
        {
          compilationContext.TemplateClassBuilder.AppendChangeOutputDepth(depth);
          compilationContext.TemplateClassBuilder.AppendSilentCode("})", true);
        };
    }
  }
}