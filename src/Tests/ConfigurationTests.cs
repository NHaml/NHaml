using System;

using Moq;

using NHaml.Compilers.Boo;
using NHaml.Compilers.CSharp2;
using NHaml.Compilers.CSharp3;
using NHaml.Compilers.IronRuby;
using NHaml.Configuration;

using NUnit.Framework;

namespace NHaml.Tests
{
  [TestFixture]
  public class ConfigurationTests
  {
    private static void CreateAndAssertTemplateCompiler(string name, Type templateCompilerType)
    {
      var sectionMock = new Mock<NHamlConfigurationSection>();
      sectionMock.Expect(s => s.TemplateCompiler)
        .Returns(() => name);

      var templateCompiler = sectionMock.Object.CreateTemplateCompiler();

      Assert.IsNotNull(templateCompiler);
      Assert.IsInstanceOfType(templateCompilerType, templateCompiler);
    }

    [Test]
    public void CanCreateIronRubyTemplateCompilerFromFullName()
    {
      CreateAndAssertTemplateCompiler("NHaml.Compilers.IronRuby.IronRubyTemplateCompiler, NHaml.Compilers.IronRuby",
        typeof(IronRubyTemplateCompiler));
    }

    [Test]
    public void CanCreateBooTemplateCompilerFromFullName()
    {
      CreateAndAssertTemplateCompiler("NHaml.Compilers.Boo.BooTemplateCompiler, NHaml.Compilers.Boo",
        typeof(BooTemplateCompiler));
    }

    [Test]
    public void CanCreateCSharp2TemplateCompilerFromFullName()
    {
      CreateAndAssertTemplateCompiler("NHaml.Compilers.CSharp2.CSharp2TemplateCompiler, NHaml",
        typeof(CSharp2TemplateCompiler));
    }

    [Test]
    public void CanCreateCSharp2TemplateCompilerFromLongName()
    {
      CreateAndAssertTemplateCompiler("CSharp2TemplateCompiler",
        typeof(CSharp2TemplateCompiler));
    }

    [Test]
    public void CanCreateCSharp2TemplateCompilerFromShortName()
    {
      CreateAndAssertTemplateCompiler("CSharp2",
        typeof(CSharp2TemplateCompiler));
    }

    [Test]
    public void CanCreateCSharp3TemplateCompilerFromFullName()
    {
      CreateAndAssertTemplateCompiler("NHaml.Compilers.CSharp3.CSharp3TemplateCompiler, NHaml",
        typeof(CSharp3TemplateCompiler));
    }

    [Test]
    public void CanCreateCSharp3TemplateCompilerFromLongName()
    {
      CreateAndAssertTemplateCompiler("CSharp3TemplateCompiler",
        typeof(CSharp3TemplateCompiler));
    }

    [Test]
    public void CanCreateCSharp3TemplateCompilerFromShortName()
    {
      CreateAndAssertTemplateCompiler("CSharp3",
        typeof(CSharp3TemplateCompiler));
    }

    [Test]
    public void CanReadTemplateCompilerFromAppSettings()
    {
      var section = NHamlConfigurationSection.GetSection();

      Assert.IsNotNull(section);
      Assert.AreEqual("CSharp3", section.TemplateCompiler);
    }

    [Test]
    public void CanReadAutoRecompileSettingFromAppSettings()
    {
      var section = NHamlConfigurationSection.GetSection();

      Assert.IsNotNull(section);
      Assert.IsTrue(section.AutoRecompile.HasValue);
      Assert.IsFalse(section.AutoRecompile.Value);
    }

    [Test]
    public void CanReadViewsAssembliesSectionFromAppSettings()
    {
      var section = NHamlConfigurationSection.GetSection();

      Assert.IsNotNull(section);
      Assert.AreEqual(1, section.Assemblies.Count);
      Assert.AreEqual("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089", section.Assemblies[0].Name);
    }

    [Test]
    public void CanReadViewsNamespacesSectionFromAppSettings()
    {
      var section = NHamlConfigurationSection.GetSection();

      Assert.IsNotNull(section);
      Assert.AreEqual(1, section.Namespaces.Count);
      Assert.AreEqual("System.Collections", section.Namespaces[0].Name);
    }
  }
}