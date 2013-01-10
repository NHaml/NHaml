using System.Web.NHaml.Crosscutting;
using System.Web.NHaml.TemplateResolution;

namespace System.Web.NHaml
{
    public class TemplateEngine : ITemplateEngine
    {
        private readonly IHamlTemplateCache _compiledTemplateCache;
        private readonly ITemplateFactoryFactory _templateFactoryFactory;

        public TemplateEngine(IHamlTemplateCache templateCache, ITemplateFactoryFactory templateFactoryFactory)
        {
            _compiledTemplateCache = templateCache;
            _templateFactoryFactory = templateFactoryFactory;
        }

        public TemplateFactory GetCompiledTemplate(ViewSource viewSource, Type templateBaseType)
        {
            return GetCompiledTemplate(new ViewSourceCollection { viewSource }, templateBaseType);
        }

        public TemplateFactory GetCompiledTemplate(ViewSourceCollection viewSourceCollection, Type templateBaseType)
        {
            Invariant.ArgumentNotNull(viewSourceCollection, "viewSourceCollection");
            Invariant.ArgumentNotNull(templateBaseType, "templateBaseType");

            templateBaseType = ProxyExtracter.GetNonProxiedType(templateBaseType);
            var className = viewSourceCollection.GetClassName();

            lock( _compiledTemplateCache )
            {
                return _compiledTemplateCache.GetOrAdd(className, viewSourceCollection[0].TimeStamp,
                    () => _templateFactoryFactory.CompileTemplateFactory(className, viewSourceCollection, templateBaseType));
            }
        }

        public ITemplateContentProvider TemplateContentProvider
        {
            set { _templateFactoryFactory.TemplateContentProvider = value; }
        }
    }
}