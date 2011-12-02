using System;
using NHaml.Compilers;
using NHaml.Parser;

namespace NHaml.Rules
{
    public class PartialMarkupRule : MarkupRule
    {
        private const string NoPartialName = "No partial name specified and template is not a layout";

        public override string Signifier
        {
            get { return "_"; }
        }

        public override  bool PerformCloseActions { get { return false; } }

        public override BlockClosingAction Render(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //var partialName = viewSourceReader.CurrentInputLine.NormalizedText.Trim();

            //if (string.IsNullOrEmpty(partialName))
            //{
            //    if (viewSourceReader.ViewSourceQueue.Count == 0)
            //    {
            //        throw new InvalidOperationException(NoPartialName);
            //    }
            //    var templatePath = viewSourceReader.ViewSourceQueue.Dequeue();
            //    viewSourceReader.MergeTemplate(templatePath, true);
            //}
            //else
            //{
            //    partialName = partialName.Insert(partialName.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1, "_");
            //    var viewSource = options.TemplateContentProvider.GetViewSource(partialName, viewSourceReader.ViewSources);
            //    viewSourceReader.ViewSourceModifiedChecks.Add(() => viewSource.IsModified);
            //    viewSourceReader.MergeTemplate(viewSource, true);
            //}

            return EmptyClosingAction;
        }
    }
}