using NHaml4.TemplateResolution;
using System;

namespace NHaml4
{
    public interface ITemplateFactoryFactory
    {
        TemplateFactory CompileTemplateFactory(string className, ViewSourceCollection viewSourceCollection, Type baseType);
    }
}
