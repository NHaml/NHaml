using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateResolution;

namespace NHaml4
{
    public interface IHamlTemplateCache
    {
        TemplateFactory GetOrAdd(string templateKey, DateTime timeStamp, Func<TemplateFactory> templateGet);
        void Clear();
    }
}
