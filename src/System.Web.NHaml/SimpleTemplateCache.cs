using System.Collections.Generic;

namespace System.Web.NHaml
{
    public class SimpleTemplateCache : IHamlTemplateCache
    {
        class TemplateFactoryCacheEntry
        {
            public DateTime TimeStamp;
            public TemplateFactory TemplateFactory;
        }

        private static readonly IDictionary<string, TemplateFactoryCacheEntry> TemplateCache = new Dictionary<string, TemplateFactoryCacheEntry>();

        public TemplateFactory GetOrAdd(string templateKey, DateTime timeStamp, Func<TemplateFactory> templateGet)
        {
            TemplateFactoryCacheEntry result;
            bool templateInCache = TemplateCache.TryGetValue(templateKey, out result);
            if (templateInCache == false || result.TimeStamp < timeStamp)
            {
                result = new TemplateFactoryCacheEntry
                {
                    TimeStamp = timeStamp,
                    TemplateFactory = templateGet()
                };
                TemplateCache[templateKey] = result;
            }

            return result.TemplateFactory;
        }

        public bool ContainsTemplate(string fileName)
        {
            return TemplateCache.ContainsKey(fileName);
        }

        public void Clear()
        {
            TemplateCache.Clear();
        }
    }
}
