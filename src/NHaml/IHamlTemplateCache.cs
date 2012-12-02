using System;

namespace NHaml
{
    public interface IHamlTemplateCache
    {
        TemplateFactory GetOrAdd(string templateKey, DateTime timeStamp, Func<TemplateFactory> templateGet);
        void Clear();
    }
}
