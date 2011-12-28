using NHaml4.TemplateResolution;

namespace NHaml4
{
    public interface ITemplateFactoryFactory
    {
        TemplateFactory CompileTemplateFactory(string className, IViewSource viewSource);
    }
}
