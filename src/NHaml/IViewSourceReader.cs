using System.Collections.Generic;
using NHaml.Rules;
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
        LinkedListNode<InputLine> CurrentNode { get;  }
        void MergeTemplate(IViewSource source, bool replace);
        MarkupRule GetRule();
        void MoveNext();
    }
}