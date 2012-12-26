using System.Web.NHaml.TemplateResolution;

namespace System.Web.NHaml
{
    public interface ITemplateEngine
    {
        TemplateFactory GetCompiledTemplate(ViewSource viewSource, Type templateBaseType);
        TemplateFactory GetCompiledTemplate(ViewSourceCollection viewSourceCollection, Type templateBaseType);
        ITemplateContentProvider TemplateContentProvider { set; }
    }
}
