using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Configuration;
using NUnit.Framework;
using NHaml.Tests.Builders;
using NHaml4.TemplateResolution;
using Moq;

namespace NHaml4.Tests
{
    [TestFixture]
    public class XmlConfigurator_Tests
    {
        [Test]
        public void GetTemplateEngine_ReturnsTemplateEngine()
        {
            var result = XmlConfigurator.GetTemplateEngine();
            Assert.That(result, Is.InstanceOf<TemplateEngine>());
        }

        [Test]
        public void GetTemplateEngine_NoNHamlConfigSpecified_ReturnsTemplateEngine()
        {
            const string configFile = "Configuration/NoNHaml.config";
            var result = XmlConfigurator.GetTemplateEngine(configFile);
            Assert.That(result, Is.InstanceOf<TemplateEngine>());
        }

        [Test]
        public void GetTemplateEngine_BasicHamlConfig_ReturnsTemplateEngine()
        {
            const string configFile = "Configuration/configWithReferencesAndImports.config";
            var result = XmlConfigurator.GetTemplateEngine(configFile);
            Assert.That(result, Is.InstanceOf<TemplateEngine>());
        }

        [Test]
        public void GetTemplateEngine_AdditionalImportsAndReferencedAssemblies_ReturnsTemplateEngine()
        {
            var importsList = new List<string>();
            var referencedAssemblies = new List<string>();
            var contentProviderMock = new Mock<ITemplateContentProvider>();

            var result = XmlConfigurator.GetTemplateEngine(contentProviderMock.Object, importsList, referencedAssemblies);
            Assert.That(result, Is.InstanceOf<TemplateEngine>());
        }
    }
}
