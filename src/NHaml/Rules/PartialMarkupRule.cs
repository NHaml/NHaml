using System;

namespace NHaml.Rules
{
    public class PartialMarkupRule : MarkupRule
    {
        private const string NoPartialName = "No partial name specified and template is not a layout";

        public override string Signifier
        {
            get { return "_"; }
        }

        public override void Process( TemplateParser templateParser )
        {
            Render( templateParser );
        }

        public override BlockClosingAction Render(TemplateParser templateParser)
        {
            var partialName = templateParser.CurrentInputLine.NormalizedText.Trim();

            if (string.IsNullOrEmpty(partialName))
            {
                if (templateParser.CurrentTemplateIndex + 1 == templateParser.MergedTemplatePaths.Count)
                {
                    throw new InvalidOperationException(NoPartialName);
                }
                var templatePath = templateParser.MergedTemplatePaths[templateParser.CurrentTemplateIndex + 1];
                if (templatePath == null)
                {
                    throw new InvalidOperationException(NoPartialName);
                }
                templateParser.MergeTemplate(templatePath, true);
                templateParser.CurrentTemplateIndex++;
            }
            else
            {
                var parentViewSource = templateParser.MergedTemplatePaths[0];
                partialName = partialName.Insert(partialName.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1, "_");
                var source = templateParser.TemplateEngine.TemplateContentProvider.GetViewSource(partialName, parentViewSource);
                templateParser.MergeTemplate(source, true);
            }

            return EmptyClosingAction;
        }
    }
}