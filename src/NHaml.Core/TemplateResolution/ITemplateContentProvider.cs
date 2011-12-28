using System.Collections.Generic;

namespace NHaml.Core.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        IViewSource GetViewSource(string templateName);
        IViewSource GetViewSource(string templateName, IList<string> templatePaths);
        IList<string> PathSources { get; set; }
        void AddPathSource(string pathSource);
    }
}