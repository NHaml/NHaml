using System.Web.NHaml.Compilers;
using System.Web.NHaml.IO;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.TemplateBase;
using System.Web.NHaml.Walkers.CodeDom;
using NUnit.Framework;
using Moq;
using NHaml.Tests.Mocks;
using System;
using System.Collections.Generic;
using NHaml.Tests.Builders;

namespace NHaml.Tests.Walkers
{
    [TestFixture]
    public class HamlDocumentWalker_Tests
    {
        [Test]
        public void Walk_TextNode_AppendsCorrectTag()
        {
            // Arrange
            var content = new HamlLine("Simple content", HamlRuleEnum.PlainText, "", 0);
            var document = HamlDocumentBuilder.Create("",
                new HamlNodeTextContainer(content));
            Type baseType = typeof(Template);

            // Act
            var builder = new ClassBuilderMock();
            new HamlDocumentWalker(builder).Walk(document, "", baseType, new List<string>());

            // Assert
            Assert.That(builder.Build(""), Is.EqualTo(content.Content));
        }

        [Test]
        public void Walk_SingleLineFile_CallsClassBuilderBuild()
        {
            // Arrange
            const string className = "ClassName";
            Type baseType = typeof(Template);
            var parser = new HamlTreeParser(new HamlFileLexer());
            var document = parser.ParseDocumentSource("Simple content", "");
            var imports = new List<string>();

            // Act
            var builder = new Mock<ITemplateClassBuilder>();

            new HamlDocumentWalker(builder.Object).Walk(document, className, baseType, imports);

            // Assert
            builder.Verify(x => x.Build(className, baseType, imports));
        }
    }
}
