using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml.Parser;
using NHaml.IO;
using NHaml.Compilers;
using NHaml.Compilers.CSharp2;
using NHaml.Tests.Builders;
using NHaml4.Walkers;
using Moq;
using NHaml.TemplateResolution;

namespace NHaml.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class CompiledTemplate_IntegrationTests
    {
        [Test]
        public void tbc()
        {
            // Arrange
            var parser = new HamlTreeParser(new HamlFileLexer());
            var walker = new CodeDomWalker();
            var compilerMock = new Mock<ITemplateFactoryCompiler>();
            string templateContent = @"This is a test";
            string expectedStatement = @"Console.WriteLn(""This is a test"");";

            var viewSource = ViewSourceBuilder.Create(templateContent);

            // Act
            var compiledTemplate = new CompiledTemplate(parser, walker, compilerMock.Object);
            compiledTemplate.CompileTemplateFactory(viewSource);

            // Assert
            compilerMock.Verify(x => x.Compile(
                    It.Is<string>(s => s.Contains(expectedStatement))
                ));
        }
    }
}
