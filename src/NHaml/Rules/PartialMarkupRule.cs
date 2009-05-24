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

        public override void Process( TemplateParser templateParser )
        {
            Render( templateParser );
        }

        public override BlockClosingAction Render( TemplateParser templateParser )
        {
            var partialName = templateParser.CurrentInputLine.NormalizedText.Trim();

            if (string.IsNullOrEmpty(partialName))
            {
                if (templateParser.CurrentTemplateIndex +1 == templateParser.MergedTemplatePaths.Count)
                {
                    throw new InvalidOperationException(Resources.NoPartialName);
                }
                var templatePath = templateParser.MergedTemplatePaths[templateParser.CurrentTemplateIndex+1];
                if (string.IsNullOrEmpty(templatePath))
                {
                    throw new InvalidOperationException(Resources.NoPartialName);
                }
                templateParser.MergeTemplate(templatePath, true);
               templateParser. CurrentTemplateIndex++;
            }
            else
            {
                var templateDirectory = Path.GetDirectoryName( templateParser.TemplatePath);

                partialName = partialName.Insert( partialName.LastIndexOf( @"\", StringComparison.OrdinalIgnoreCase ) + 1, "_" );

                var partialTemplatePath = Path.Combine( templateDirectory, partialName + ".haml" );

                if( !File.Exists( partialTemplatePath ) )
                {
                    partialTemplatePath = Path.Combine( templateDirectory, @"..\" + partialName + ".haml" );
                }

                templateParser.MergeTemplate( partialTemplatePath, true );
            }

            return EmptyClosingAction;
        }
    }
}