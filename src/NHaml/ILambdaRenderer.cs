using System.Text.RegularExpressions;

namespace NHaml
{
  public interface ILambdaRenderer
  {
    string Render(string codeLine, Match lambdaMatch);
  }
}