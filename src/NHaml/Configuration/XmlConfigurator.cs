using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web.NHaml.Compilers;
using System.Web.NHaml.IO;
using System.Web.NHaml.Parser;
using System.Web.NHaml.TemplateResolution;
using System.Web.NHaml.Walkers.CodeDom;

namespace System.Web.NHaml.Configuration
{
    public static class XmlConfigurator
    {
        public static TemplateEngine GetTemplateEngine()
        {
            return GetTemplateEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
        }

        public static TemplateEngine GetTemplateEngine(ITemplateContentProvider templateContentProvider, IEnumerable<string> imports, IEnumerable<string> referencedAssemblies)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration();
            return GetTemplateEngine(templateContentProvider, nhamlConfiguration, imports, referencedAssemblies);
        }
        
        public static TemplateEngine GetTemplateEngine(string configFile)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration(configFile);
            return GetTemplateEngine(new FileTemplateContentProvider(), nhamlConfiguration, new List<string>(), new List<string>());
        }

        private static TemplateEngine GetTemplateEngine(ITemplateContentProvider templateContentProvider, NHamlConfigurationSection nhamlConfiguration, IEnumerable<string> imports, IEnumerable<string> referencedAssemblies)
        {
            var templateCache = new SimpleTemplateCache();

            var templateFactoryFactory = new TemplateFactoryFactory(
                templateContentProvider,
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CodeDomClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()),
                nhamlConfiguration.ImportsList.Concat(imports),
                nhamlConfiguration.ReferencedAssembliesList.Concat(referencedAssemblies));

            return new TemplateEngine(templateCache, templateFactoryFactory);
        }
    }
}
