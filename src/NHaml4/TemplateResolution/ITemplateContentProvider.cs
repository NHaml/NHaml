using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHaml4.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        IViewSource GetViewSource(string templateName);
        IViewSource GetViewSource( string templatePath, IList<IViewSource> parentViewSourceList );
        IEnumerable<string> PathSources { get; }
        void AddPathSource(string pathSource);
    }
}