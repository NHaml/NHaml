using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Configuration;
using System.IO;

namespace NHaml4.Tests.Configuration
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
            Assert.That(config.ReferencedAssembliesList.First(), Is.StringEnding("NHaml4.Tests.dll"));
        }

        [Test]
        public void GetConfiguration_ConfigWithReferencesAndImports_ImportListContainsCorrectElement()
        {
            var config = NHamlConfigurationSection.GetConfiguration("Configuration/configWithReferencesAndImports.config");
            Assert.That(config.ImportsList.First(), Is.EqualTo("NHaml4.Tests.Configuration, NHaml4.Tests"));
        }
    }
}
