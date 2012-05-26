using System;
using NHaml4.TemplateResolution;

namespace NHaml4
{
    public interface ITemplateEngine
    {
        TemplateFactory GetCompiledTemplate(ViewSource viewSource, Type templateBaseType);
        TemplateFactory GetCompiledTemplate(ViewSourceCollection viewSourceCollection, Type templateBaseType);
        ITemplateContentProvider TemplateContentProvider { set; }
    }
}
