using System;
using NHaml.TemplateResolution;

namespace NHaml
{
    public interface ITemplateEngine
    {
        TemplateFactory GetCompiledTemplate(ViewSource viewSource, Type templateBaseType);
        TemplateFactory GetCompiledTemplate(ViewSourceCollection viewSourceCollection, Type templateBaseType);
        ITemplateContentProvider TemplateContentProvider { set; }
    }
}
