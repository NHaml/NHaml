using System.Web.NHaml.TemplateResolution;

namespace System.Web.NHaml
{
    public interface ITemplateFactoryFactory
    {
        TemplateFactory CompileTemplateFactory(string className, ViewSourceCollection viewSourceCollection, Type baseType);
        ITemplateContentProvider TemplateContentProvider { set; }
    }
}
