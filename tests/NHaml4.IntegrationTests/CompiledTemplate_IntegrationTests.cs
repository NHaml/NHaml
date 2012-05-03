using NHaml4;
using NUnit.Framework;
using NHaml.Tests.Builders;
using System.IO;
using NHaml4.IO;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NHaml4.Compilers;
using NHaml4.TemplateBase;
using NHaml4.Compilers.Abstract;
using System.Collections.Generic;
using NHaml4.TemplateResolution;

namespace NHaml.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class CompiledTemplate_IntegrationTests
    {
        [Test]
        public void SimpleIntegrationTest()
        {
            // Arrange
            string templateContent = @"This is a test";

            var viewSource = ViewSourceBuilder.Create(templateContent);

            // Act
            var templateFactoryFactory = new TemplateFactoryFactory(
                new FileTemplateContentProvider(),
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CodeDomClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()),
                new List<string>(),
                new List<string>());

            var templateFactory = templateFactoryFactory.CompileTemplateFactory(viewSource.GetClassName(), viewSource);
            Template template = templateFactory.CreateTemplate();
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual("This is a test", textWriter.ToString());
        }

        [Test]
        public void PartialsIntegrationTest()
        {
            // Arrange
            string template1Content = "This is a test\n_File2";
            string template2Content = "Of a partial";

            var viewSourceList = new ViewSourceCollection {
                ViewSourceBuilder.Create(template1Content),
                ViewSourceBuilder.Create(template2Content, "File2")
            };

            // Act
            var templateFactoryFactory = new TemplateFactoryFactory(
                new FileTemplateContentProvider(),
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CodeDomClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()),
                new List<string>(),
                new List<string>());

            var templateFactory = templateFactoryFactory.CompileTemplateFactory(viewSourceList.GetClassName(), viewSourceList);
            Template template = templateFactory.CreateTemplate();
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual("This is a test\nOf a partial", textWriter.ToString());
        }
    }
}
