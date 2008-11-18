using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NHaml.Rules
{
  public class PartialMarkupRule : MarkupRule
  {
    private static readonly Regex _partialRegex
      = new Regex(@"^\s*([\w-\\]+)\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

    public override char Signifier
    {
      get { return '_'; }
    }

    public override void Process(TemplateParser templateParser)
    {
      Render(templateParser);
    }

    public override BlockClosingAction Render(TemplateParser templateParser)
    {
      var match = _partialRegex.Match(templateParser.CurrentInputLine.NormalizedText);

      if (match.Success)
      {
        var templateDirectory = Path.GetDirectoryName(templateParser.TemplatePath);

        var partialName = match.Groups[1].Value;

        partialName = partialName.Insert(partialName.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1, "_");

        var partialTemplatePath = Path.Combine(templateDirectory, partialName + ".haml");

        if (!File.Exists(partialTemplatePath))
        {
          partialTemplatePath = Path.Combine(templateDirectory, @"..\" + partialName + ".haml");
        }

        templateParser.MergeTemplate(partialTemplatePath);
      }
      else if (!string.IsNullOrEmpty(templateParser.LayoutTemplatePath))
      {
        templateParser.MergeTemplate(templateParser.TemplatePath);
      }

      return null;
    }
  }
}