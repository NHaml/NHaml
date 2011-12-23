using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NHaml.Tests.Builders;
using Moq;
using System.IO;
using NHaml4.TemplateResolution;
using NHaml.IO;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NHaml4.Compilers;
using NHaml4.Compilers.CSharp2;
using NHaml4.TemplateBase;

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
            var compiledTemplate = new TemplateFactoryFactory(
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CSharp2TemplateClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()));

            var templateFactory = compiledTemplate.CompileTemplateFactory(viewSource);
            Template template = templateFactory.CreateTemplate();
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual("This is a test", textWriter.ToString());
        }
    }
}
