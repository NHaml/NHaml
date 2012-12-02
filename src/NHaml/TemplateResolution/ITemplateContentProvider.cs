namespace NHaml.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        ViewSource GetViewSource(string templateName);
    }
}