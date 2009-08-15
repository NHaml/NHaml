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
                if (templateParser.ViewSourceQueue.Count == 0)
                {
                    throw new InvalidOperationException(NoPartialName);
                }
                var templatePath = templateParser.ViewSourceQueue.Dequeue();
                templateParser.MergeTemplate(templatePath, true);
            }
            else
            {
                partialName = partialName.Insert(partialName.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1, "_");
                var viewSource = templateParser.TemplateEngine.TemplateContentProvider.GetViewSource(partialName, templateParser.ViewSources);
                templateParser.ViewSourceModifiedChecks.Add(() => viewSource.IsModified);
                templateParser.MergeTemplate(viewSource, true);
            }

            return EmptyClosingAction;
        }
    }
}