using NHaml4.TemplateResolution;
using System;
using System.Collections.Generic;

namespace NHaml4
{
    public interface ITemplateFactoryFactory
    {
        TemplateFactory CompileTemplateFactory(string className, IViewSource viewSource, Type baseType);
    }
}
