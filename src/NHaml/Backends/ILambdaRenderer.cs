using System.Text.RegularExpressions;

namespace NHaml.BackEnds
{
  public interface ILambdaRenderer
  {
    string Render(string codeLine, Match lambdaMatch);
  }
}