using System;
using System.Text.RegularExpressions;

namespace NHaml.BackEnds.CSharp3
{
  internal class CSharp3LambdaRenderer : ILambdaRenderer
  {
    public string Render(string codeLine, Match lambdaMatch)
    {
      return codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2)
        + (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ")
          + lambdaMatch.Groups[2].Captures[0].Value + " => {";
    }
  }
}