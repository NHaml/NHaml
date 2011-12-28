using NUnit.Framework;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NHaml4.Tests.Walkers.CodeDom;

namespace NHaml4.Tests.Walkers
{
    [TestFixture]
    public class HamlDocumentWalker_Tests
    {
        private Mock<ITemplateClassBuilder> _classBuilder;
        private HamlDocumentWalker _walker;

        [SetUp]
        public void SetUp()
        {
            _classBuilder = new Mock<ITemplateClassBuilder>();
            _walker = new HamlDocumentWalker(_classBuilder.Object);
        }

        [Test]
        public void Walk_TextNode_AppendsCorrectTag()
        {
            // Arrange
            const string content = "Simple content";
            var document = new HamlDocument { new HamlNodeText(content) };

            // Act
            _walker.Walk(document, content);

            // Assert
            _classBuilder.Verify(x => x.Append(content));
        }

        [Test]
        public void Walk_SingleLineFile_CallsClassBuilderBuild()
        {
            // Arrange
            const string className = "ClassName";
            var document = new HamlTreeParser(new NHaml4.IO.HamlFileLexer()).ParseDocumentSource("Simple content");
            
            // Act
            _walker.Walk(document, className);

            // Assert
            _classBuilder.Verify(x => x.Build(className));
        }
    }
}
