namespace NHaml4.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        ViewSource GetViewSource(string templateName);
    }
}