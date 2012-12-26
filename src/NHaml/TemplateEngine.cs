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

        //public TemplateFactory GetCompiledTemplate(ITemplateContentProvider contentProvider, string templatePath, Type templateBaseType)
        //{
        //    Invariant.ArgumentNotNull(contentProvider, "contentProvider");

        //    var viewSourceCollection = new ViewSourceCollection { contentProvider.GetViewSource(templatePath) };
        //    return GetCompiledTemplate(viewSourceCollection, templateBaseType);
        //}

        //public TemplateFactory GetCompiledTemplate(ITemplateContentProvider contentProvider, string templatePath, string masterPath, Type templateBaseType)
        //{
        //    Invariant.ArgumentNotNull(contentProvider, "contentProvider");

        //    var viewSourceCollection = GetViewSourceCollection(contentProvider, templatePath, masterPath);
        //    return GetCompiledTemplate(viewSourceCollection, templateBaseType);
        //}

        private static ViewSourceCollection GetViewSourceCollection(ITemplateContentProvider contentProvider, string templatePath, string masterPath)
        {
            return new ViewSourceCollection {
                contentProvider.GetViewSource(masterPath),
                contentProvider.GetViewSource(templatePath)
            };
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