using System;

namespace NHaml4
{
    public interface ITemplateFactoryFactory
    {
        TemplateFactory CompileTemplateFactory(ViewSourceList viewSourceList);
    }
}
