using System.Collections.Generic;
using System.Linq;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NHaml4.IO;
using NHaml4.Compilers;
using System.Configuration;

namespace NHaml4.Configuration
{
    public static class XmlConfigurator
    {
        public static TemplateEngine GetTemplateEngine()
        {
            return GetTemplateEngine(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
        }

        public static TemplateEngine GetTemplateEngine(IEnumerable<string> imports, IEnumerable<string> referencedAssemblies)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration();
            return GetTemplateEngine(nhamlConfiguration, imports, referencedAssemblies);
        }
        
        public static TemplateEngine GetTemplateEngine(string configFile)
        {
            var nhamlConfiguration = NHamlConfigurationSection.GetConfiguration(configFile);
            return GetTemplateEngine(nhamlConfiguration, new List<string>(), new List<string>());
        }

        private static TemplateEngine GetTemplateEngine(NHamlConfigurationSection nhamlConfiguration, IEnumerable<string> imports, IEnumerable<string> referencedAssemblies)
        {
            var templateCache = new SimpleTemplateCache();

            var templateFactoryFactory = new TemplateFactoryFactory(
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CodeDomClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()),
                nhamlConfiguration.ImportsList.Concat(imports),
                nhamlConfiguration.ReferencedAssembliesList.Concat(referencedAssemblies));

            return new TemplateEngine(templateCache, templateFactoryFactory);
        }
    }
}
