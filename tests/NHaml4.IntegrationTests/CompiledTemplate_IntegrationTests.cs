using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml.IO;
using NHaml4.Compilers;
using NHaml4.Compilers.CSharp2;
using NHaml.Tests.Builders;
using NHaml4.Walkers;
using Moq;
using NHaml4.TemplateResolution;
using System.IO;

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
            var parser = new HamlTreeParser(new HamlFileLexer());
            var walker = new CodeDomWalker(new CSharp2TemplateClassBuilder());
            var compilerMock = new Mock<ITemplateFactoryCompiler>();
            string templateContent = @"This is a test";

            var viewSource = ViewSourceBuilder.Create(templateContent);

            // Act
            var compiledTemplate = new CompiledTemplate(parser, walker, compilerMock.Object);
            compiledTemplate.CompileTemplateFactory(viewSource);
            Template template = compiledTemplate.CreateInstance();
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual("This is a test", textWriter.ToString());
        }
    }
}
