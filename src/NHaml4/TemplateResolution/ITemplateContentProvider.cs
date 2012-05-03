using System.Collections.Generic;

namespace NHaml4.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        IViewSource GetViewSource(string templateName);
        IViewSource GetViewSource( string templatePath, IEnumerable<IViewSource> parentViewSourceList );
        IEnumerable<string> PathSources { get; }
        void AddPathSource(string pathSource);
    }
}