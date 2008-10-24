
using System;

using Moq;

using NHaml.Backends;
using NHaml.Backends.Boo;
using NHaml.Backends.CSharp2;
using NHaml.Backends.CSharp3;
using NHaml.Configuration;

using NUnit.Framework;

namespace NHaml.Tests.Configuration
{
  [TestFixture]
  public class ConfigurationTester
  {
    private void CreateAndAssertCompilerBackend(string name,
      Type compilerBackendType)
    {
      var sectionMock = new Mock<NHamlSection>();
      sectionMock.Expect(s => s.CompilerBackend)
        .Returns(() => name);

      ICompilerBackend backend = sectionMock.Object.CreateCompilerBackend();

      Assert.IsNotNull(backend);
      Assert.IsInstanceOfType(compilerBackendType, backend);
    }

    [Test]
    public void CanCreateBooCompilerBackendFromFullName()
    {
      CreateAndAssertCompilerBackend("NHaml.Backends.Boo.BooCompilerBackend, NHaml.Backends.Boo",
        typeof(BooCompilerBackend));
    }

    [Test]
    public void CanCreateCSharp2CompilerBackendFromFullName()
    {
      CreateAndAssertCompilerBackend("NHaml.Backends.CSharp2.CSharp2CompilerBackend, NHaml",
        typeof(CSharp2CompilerBackend));
    }

    [Test]
    public void CanCreateCSharp2CompilerBackendFromLongName()
    {
      CreateAndAssertCompilerBackend("CSharp2CompilerBackend",
        typeof(CSharp2CompilerBackend));
    }

    [Test]
    public void CanCreateCSharp2CompilerBackendFromShortName()
    {
      CreateAndAssertCompilerBackend("CSharp2",
        typeof(CSharp2CompilerBackend));
    }

    [Test]
    public void CanCreateCSharp3CompilerBackendFromFullName()
    {
      CreateAndAssertCompilerBackend("NHaml.Backends.CSharp3.CSharp3CompilerBackend, NHaml",
        typeof(CSharp3CompilerBackend));
    }

    [Test]
    public void CanCreateCSharp3CompilerBackendFromLongName()
    {
      CreateAndAssertCompilerBackend("CSharp3CompilerBackend",
        typeof(CSharp3CompilerBackend));
    }

    [Test]
    public void CanCreateCSharp3CompilerBackendFromShortName()
    {
      CreateAndAssertCompilerBackend("CSharp3",
        typeof(CSharp3CompilerBackend));
    }

    [Test]
    public void CanReadCompilerBackendFromAppSettings()
    {
      NHamlSection section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.AreEqual("CSharp2", section.CompilerBackend);
    }

    [Test]
    public void CanReadProductionSettingFromAppSettings()
    {
      NHamlSection section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.IsTrue(section.Production);
    }

    [Test]
    public void CanReadViewsAssembliesSectionFromAppSettings()
    {
      NHamlSection section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.AreEqual(1, section.Assemblies.Count);
      Assert.AreEqual("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089", section.Assemblies[0].Name);
    }

    [Test]
    public void CanReadViewsNamespacesSectionFromAppSettings()
    {
      NHamlSection section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.AreEqual(1, section.Namespaces.Count);
      Assert.AreEqual("System.Collections", section.Namespaces[0].Name);
    }
  }
}