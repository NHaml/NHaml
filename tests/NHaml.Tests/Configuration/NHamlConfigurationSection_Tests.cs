using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml.Configuration;
using System.IO;

namespace NHaml.Tests.Configuration
{
    [TestFixture]
    class NHamlConfigurationSection_Tests
    {
        [Test]
        public void GetConfiguration_EmptyFileName_ThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => NHamlConfigurationSection.GetConfiguration(""));
        }

        [Test]
        public void GetConfiguration_ConfigWithReferencesAndImports_ReferencedAssemblyListContainsCorrectElement()
        {
            var config = NHamlConfigurationSection.GetConfiguration("Configuration/configWithReferencesAndImports.config");
            Assert.That(config.ReferencedAssembliesList.First().ToLower(), Is.StringEnding("nhaml.tests.dll"));
        }

        [Test]
        public void GetConfiguration_ConfigWithReferencesAndImports_ImportListContainsCorrectElement()
        {
            var config = NHamlConfigurationSection.GetConfiguration("Configuration/configWithReferencesAndImports.config");
            Assert.That(config.ImportsList.First(), Is.EqualTo("NHaml.Tests.Configuration, NHaml.Tests"));
        }
    }
}
