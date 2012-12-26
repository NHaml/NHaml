namespace System.Web.NHaml.TemplateResolution
{
    public interface ITemplateContentProvider
    {
        ViewSource GetViewSource(string templateName);
    }
}