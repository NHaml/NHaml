using System;
using Moq;
using NHaml.Compilers.Boo;
using NHaml.Compilers.CSharp2;
using NHaml.Compilers.CSharp3;
using NHaml.Compilers.FSharp;
using NHaml.Compilers.IronRuby;
using NHaml.Compilers.VisualBasic;
using NHaml.Configuration;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        private static void CreateAndAssertTemplateCompiler( string name, Type templateCompilerType )
        {
            var sectionMock = new Mock<NHamlConfigurationSection>();
            sectionMock.Expect( s => s.TemplateCompiler )
              .Returns( () => name );

            var templateCompiler = sectionMock.Object.CreateTemplateCompiler();

            Assert.IsNotNull( templateCompiler );
            Assert.IsInstanceOfType( templateCompilerType, templateCompiler );
        }

        [Test]
        public void CanCreateIronRubyTemplateCompilerFromFullName()
        {
            CreateAndAssertTemplateCompiler( "NHaml.Compilers.IronRuby.IronRubyTemplateCompiler, NHaml.Compilers.IronRuby",
              typeof( IronRubyTemplateCompiler ) );
        }

        [Test]
        public void CanCreateVisualBasiTemplateCompilerFromFullName()
        {
            CreateAndAssertTemplateCompiler("NHaml.Compilers.VisualBasic.VisualBasicTemplateCompiler, NHaml.Compilers.VisualBasic",
              typeof(VisualBasicTemplateCompiler));
        }

        [Test]
        public void CanCreateBooTemplateCompilerFromFullName()
        {
            CreateAndAssertTemplateCompiler( "NHaml.Compilers.Boo.BooTemplateCompiler, NHaml.Compilers.Boo",
              typeof( BooTemplateCompiler ) );
        }


        [Test]
        public void CanCreateFSharpTemplateCompilerFromFullName()
        {
            CreateAndAssertTemplateCompiler("NHaml.Compilers.FSharp.FSharpTemplateCompiler, NHaml.Compilers.FSharp",
              typeof(FSharpTemplateCompiler));
        }

        [Test]
        public void CanCreateCSharp2TemplateCompilerFromFullName()
        {
            CreateAndAssertTemplateCompiler( "NHaml.Compilers.CSharp2.CSharp2TemplateCompiler, NHaml",
              typeof( CSharp2TemplateCompiler ) );
        }

        [Test]
        public void CanCreateCSharp2TemplateCompilerFromLongName()
        {
            CreateAndAssertTemplateCompiler( "CSharp2TemplateCompiler",
              typeof( CSharp2TemplateCompiler ) );
        }

        [Test]
        public void CanCreateCSharp2TemplateCompilerFromShortName()
        {
            CreateAndAssertTemplateCompiler( "CSharp2",
              typeof( CSharp2TemplateCompiler ) );
        }

        [Test]
        public void CanCreateCSharp3TemplateCompilerFromFullName()
        {
            CreateAndAssertTemplateCompiler( "NHaml.Compilers.CSharp3.CSharp3TemplateCompiler, NHaml",
              typeof( CSharp3TemplateCompiler ) );
        }

        [Test]
        public void CanCreateCSharp3TemplateCompilerFromLongName()
        {
            CreateAndAssertTemplateCompiler( "CSharp3TemplateCompiler",
              typeof( CSharp3TemplateCompiler ) );
        }

        [Test]
        public void CanCreateCSharp3TemplateCompilerFromShortName()
        {
            CreateAndAssertTemplateCompiler( "CSharp3",
              typeof( CSharp3TemplateCompiler ) );
        }

        [Test]
        public void CanReadTemplateCompilerFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.AreEqual( "CSharp3", section.TemplateCompiler );
        }

        [Test]
        public void CanReadAutoRecompileSettingFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.IsTrue( section.AutoRecompile.HasValue );
            Assert.IsFalse( section.AutoRecompile.Value );
        }

        [Test]
        public void CanReadUseTabsSettingFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.IsTrue( section.UseTabs.HasValue );
            Assert.IsFalse( section.UseTabs.Value );
        }

        [Test]
        public void CanReadIndentSizeSettingFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.IsTrue( section.IndentSize.HasValue );
            Assert.AreEqual( 2, section.IndentSize.Value );
        }
        [Test]
        public void CanReadOutputDebugFilesSettingFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.IsTrue( section.OutputDebugFiles.HasValue );
            Assert.AreEqual(true, section.OutputDebugFiles.Value);
        }

        [Test]
        public void CanReadEscapeHtmlFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.IsTrue( section.EncodeHtml.HasValue );
            Assert.IsFalse( section.EncodeHtml.Value );
        }

        [Test]
        public void CanReadViewsAssembliesSectionFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.AreEqual( 1, section.Assemblies.Count );
            Assert.AreEqual( "System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089", section.Assemblies[0].Name );
        }

        [Test]
        public void CanReadViewsNamespacesSectionFromAppSettings()
        {
            var section = NHamlConfigurationSection.GetSection();

            Assert.IsNotNull( section );
            Assert.AreEqual( 1, section.Namespaces.Count );
            Assert.AreEqual( "System.Collections", section.Namespaces[0].Name );
        }
    }
}