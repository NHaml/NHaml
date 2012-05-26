using System.Collections.Generic;

namespace NHaml4.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        ViewSource GetViewSource(string templateName);
    }
}