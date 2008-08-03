using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NHaml.Rules
{
  public class PartialMarkupRule : MarkupRule
  {
    private static readonly Regex _partialRegex
      = new Regex(@"^\s*([\w-\\]+)$", RegexOptions.Compiled | RegexOptions.Singleline);

    public override char Signifier
    {
      get { return '_'; }
    }

    public override void Process(CompilationContext compilationContext)
    {
      Render(compilationContext);
    }

    public override BlockClosingAction Render(CompilationContext compilationContext)
    {
      var match = _partialRegex.Match(compilationContext.CurrentInputLine.NormalizedText);

      if (match.Success)
      {
        var templateDirectory
          = Path.GetDirectoryName(compilationContext.TemplatePath);

        var partialName = match.Groups[1].Value;
        partialName = partialName.Insert(partialName.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1, "_");

        var partialTemplatePath
          = Path.Combine(templateDirectory, partialName + ".haml");

        if (!File.Exists(partialTemplatePath))
        {
          partialTemplatePath
            = Path.Combine(templateDirectory, @"..\" + partialName + ".haml");
        }

        compilationContext.MergeTemplate(partialTemplatePath);
      }
      else if (!string.IsNullOrEmpty(compilationContext.LayoutPath))
      {
        compilationContext.MergeTemplate(compilationContext.TemplatePath);
      }

      return null;
    }
  }
}