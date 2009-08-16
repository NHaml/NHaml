using System.Collections.Generic;
using NHaml.TemplateResolution;

namespace NHaml
{
    public interface IViewSourceReader
    {
        InputLine CurrentInputLine { get; }
        Queue<IViewSource> ViewSourceQueue { get; }
        string NextIndent { get; }
        IList<IViewSource> ViewSources { get; }
        IList<Func<bool>> ViewSourceModifiedChecks { get; }
        bool IsBlock { get; }
        InputLine NextInputLine { get; }
        void MergeTemplate(IViewSource source, bool replace);
    }
}