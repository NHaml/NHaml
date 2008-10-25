using System;

using Moq;

using NHaml.BackEnds.Boo;
using NHaml.BackEnds.CSharp2;
using NHaml.BackEnds.CSharp3;
using NHaml.Configuration;

using NUnit.Framework;

namespace NHaml.Tests.Configuration
{
  [TestFixture]
  public class ConfigurationTester
  {
    private static void CreateAndAssertCompilerBackEnd(string name,
      Type compilerBackEndType)
    {
      var sectionMock = new Mock<NHamlSection>();
      sectionMock.Expect(s => s.CompilerBackEnd)
        .Returns(() => name);

      var backEnd = sectionMock.Object.CreateCompilerBackEnd();

      Assert.IsNotNull(backEnd);
      Assert.IsInstanceOfType(compilerBackEndType, backEnd);
    }

    [Test]
    public void CanCreateBooCompilerBackEndFromFullName()
    {
      CreateAndAssertCompilerBackEnd("NHaml.BackEnds.Boo.BooCompilerBackEnd, NHaml.BackEnds.Boo",
        typeof(BooCompilerBackEnd));
    }

    [Test]
    public void CanCreateCSharp2CompilerBackEndFromFullName()
    {
      CreateAndAssertCompilerBackEnd("NHaml.BackEnds.CSharp2.CSharp2CompilerBackEnd, NHaml",
        typeof(CSharp2CompilerBackEnd));
    }

    [Test]
    public void CanCreateCSharp2CompilerBackEndFromLongName()
    {
      CreateAndAssertCompilerBackEnd("CSharp2CompilerBackEnd",
        typeof(CSharp2CompilerBackEnd));
    }

    [Test]
    public void CanCreateCSharp2CompilerBackEndFromShortName()
    {
      CreateAndAssertCompilerBackEnd("CSharp2",
        typeof(CSharp2CompilerBackEnd));
    }

    [Test]
    public void CanCreateCSharp3CompilerBackEndFromFullName()
    {
      CreateAndAssertCompilerBackEnd("NHaml.BackEnds.CSharp3.CSharp3CompilerBackEnd, NHaml",
        typeof(CSharp3CompilerBackEnd));
    }

    [Test]
    public void CanCreateCSharp3CompilerBackEndFromLongName()
    {
      CreateAndAssertCompilerBackEnd("CSharp3CompilerBackEnd",
        typeof(CSharp3CompilerBackEnd));
    }

    [Test]
    public void CanCreateCSharp3CompilerBackEndFromShortName()
    {
      CreateAndAssertCompilerBackEnd("CSharp3",
        typeof(CSharp3CompilerBackEnd));
    }

    [Test]
    public void CanReadCompilerBackEndFromAppSettings()
    {
      var section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.AreEqual("CSharp2", section.CompilerBackEnd);
    }

    [Test]
    public void CanReadProductionSettingFromAppSettings()
    {
      var section = NHamlSection.Read();

      Assert.IsNotNull(section);
      Assert.IsTrue(section.Production);
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