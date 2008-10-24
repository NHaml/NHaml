using System.Text.RegularExpressions;

namespace NHaml.Backends
{
  public interface ILambdaRenderer
  {
    string Render(string codeLine, Match lambdaMatch);
  }
}