using System.Collections.Generic;
using System.Linq;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NHaml4.IO;
using NHaml4.Compilers;
using System.Configuration;
using NHaml4.TemplateResolution;

namespace NHaml4.Configuration
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
