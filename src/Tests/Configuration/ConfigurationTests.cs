using NHaml.Configuration;

using NUnit.Framework;

namespace NHaml.Tests.Configuration
{
  [TestFixture]
  public class ConfigurationTester
  {
    [Test]
    public void CanReadProductionSettingFromAppSettings()
    {
      var section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.IsTrue(section.Production);
    }

    [Test]
    public void CanReadCompilerVersionFromAppSettings()
    {
      var section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.AreEqual("2.0", section.CompilerVersion);
    }

    [Test]
    public void CanReadViewsAssembliesSectionFromAppSettings()
    {
      var section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.AreEqual(1, section.Assemblies.Count);
      Assert.AreEqual("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089", section.Assemblies[0].Name);
    }

    [Test]
    public void CanReadViewsNamespacesSectionFromAppSettings()
    {
      var section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.AreEqual(1, section.Namespaces.Count);
      Assert.AreEqual("System.Collections", section.Namespaces[0].Name);
    }
  }
}