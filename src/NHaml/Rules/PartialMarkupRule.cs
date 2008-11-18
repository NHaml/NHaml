using System;
using System.IO;

using NHaml.Properties;

namespace NHaml.Rules
{
  public class PartialMarkupRule : MarkupRule
  {
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
      var partialName = templateParser.CurrentInputLine.NormalizedText.Trim();

      if (string.IsNullOrEmpty(partialName))
      {
        if (!string.IsNullOrEmpty(templateParser.LayoutTemplatePath))
        {
          templateParser.MergeTemplate(templateParser.TemplatePath);
        }
        else
        {
          throw new InvalidOperationException(Resources.NoPartialName);
        }
      }
      else
      {
        var templateDirectory = Path.GetDirectoryName(templateParser.TemplatePath);

        partialName = partialName.Insert(partialName.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1, "_");

        var partialTemplatePath = Path.Combine(templateDirectory, partialName + ".haml");

        if (!File.Exists(partialTemplatePath))
        {
          partialTemplatePath = Path.Combine(templateDirectory, @"..\" + partialName + ".haml");
        }

        templateParser.MergeTemplate(partialTemplatePath);
      }

      return null;
    }
  }
}