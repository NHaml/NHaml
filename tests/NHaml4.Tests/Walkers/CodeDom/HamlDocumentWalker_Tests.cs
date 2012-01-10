using NUnit.Framework;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NHaml4.Tests.Walkers.CodeDom;
using NHaml4.Parser.Rules;
using NHaml4.IO;
using NHaml4.Tests.Mocks;

namespace NHaml4.Tests.Walkers
{
    [TestFixture]
    public class HamlDocumentWalker_Tests
    {
        [Test]
        public void Walk_TextNode_AppendsCorrectTag()
        {
            // Arrange
            var content = new HamlLine("Simple content");
            var document = new HamlDocument { new HamlNodeText(content) };

            // Act
            var builder = new ClassBuilderMock();
            new HamlDocumentWalker(builder).Walk(document, "");

            // Assert
            Assert.That(builder.Build(""), Is.EqualTo(content.Content));
        }

        [Test]
        public void Walk_SingleLineFile_CallsClassBuilderBuild()
        {
            // Arrange
            const string className = "ClassName";
            var document = new HamlTreeParser(new NHaml4.IO.HamlFileLexer()).ParseDocumentSource("Simple content");
            
            // Act
            var builder = new Mock<ITemplateClassBuilder>();
            
            new HamlDocumentWalker(builder.Object).Walk(document, className);

            // Assert
            builder.Verify(x => x.Build(className));
        }
    }
}
