using System;

namespace NHaml4
{
    public interface IHamlTemplateCache
    {
        TemplateFactory GetOrAdd(string templateKey, DateTime timeStamp, Func<TemplateFactory> templateGet);
        void Clear();
    }
}
