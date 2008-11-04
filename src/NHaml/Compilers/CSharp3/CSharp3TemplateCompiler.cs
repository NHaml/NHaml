using System;
using System.Text.RegularExpressions;

using NHaml.Compilers.CSharp2;

namespace NHaml.Compilers.CSharp3
{
  public sealed class CSharp3TemplateCompiler : CSharp2TemplateCompiler
  {
    public override string TranslateLambda(string codeLine, Match lambdaMatch)
    {
      return codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2)
        + (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ")
          + lambdaMatch.Groups[2].Captures[0].Value + " => {";
    }

    protected override void RenderAttributesCore(TemplateParser templateParser, string attributes)
    {
      templateParser.TemplateClassBuilder
        .AppendCode("Utility.RenderAttributes(new {" + attributes + "})");
    }

    internal override CSharp2TemplateTypeBuilder CreateTemplateTypeBuilder(TemplateEngine templateEngine)
    {
      return new CSharp3TemplateTypeBuilder(templateEngine);
    }
  }
}