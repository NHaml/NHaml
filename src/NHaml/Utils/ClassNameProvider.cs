using System.Text.RegularExpressions;

namespace NHaml.Utils
{
  public static class ClassNameProvider
  {
    private static readonly Regex _pathCleaner = new Regex(
      @"[^0-9a-z_]",
      RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

    public static string MakeClassName(string templatePath)
    {
      Invariant.ArgumentNotEmpty(templatePath, "templatePath");

      return _pathCleaner.Replace(templatePath, "_").Trim('_');
    }
  }
}