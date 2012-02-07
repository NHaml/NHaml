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
                new HamlDocumentWalker(new CodeDomClassBuilder(new System.Collections.Generic.List<string>())),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()));

            var templateFactory = compiledTemplate.CompileTemplateFactory(viewSource.GetClassName(), viewSource);
            Template template = templateFactory.CreateTemplate();
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual("This is a test", textWriter.ToString());
        }
    }
}
