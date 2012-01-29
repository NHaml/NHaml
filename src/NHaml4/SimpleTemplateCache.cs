using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4
{
    public class SimpleTemplateCache : IHamlTemplateCache
    {
        class TemplateFactoryCacheEntry
        {
            public DateTime TimeStamp;
            public TemplateFactory TemplateFactory;
        }

        private static readonly IDictionary<string, TemplateFactoryCacheEntry> _templateCache = new Dictionary<string, TemplateFactoryCacheEntry>();

        public TemplateFactory GetOrAdd(string templateKey, DateTime timeStamp, Func<TemplateFactory> templateGet)
        {
            TemplateFactoryCacheEntry result;
            bool templateInCache = _templateCache.TryGetValue(templateKey, out result);
            if (templateInCache == false || result.TimeStamp < timeStamp)
            {
                result = new TemplateFactoryCacheEntry
                {
                    TimeStamp = timeStamp,
                    TemplateFactory = templateGet()
                };
                _templateCache[templateKey] = result;
            }

            return result.TemplateFactory;
        }

        public bool ContainsTemplate(string fileName)
        {
            return _templateCache.ContainsKey(fileName);
        }

        public void Clear()
        {
            _templateCache.Clear();
        }
    }
}
