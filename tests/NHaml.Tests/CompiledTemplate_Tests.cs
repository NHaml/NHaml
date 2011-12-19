using NUnit.Framework;
using NHaml4.Parser;
using Moq;
using NHaml.Tests.Builders;
using NHaml4.Compilers;
using NHaml4.TemplateResolution;
using System.Collections.Generic;
using NHaml4;
using NHaml4.Walkers;
using NHaml.Utils;

namespace NHaml.Tests
{
    [TestFixture]
    public class CompiledTemplate_Tests
    {
        private Mock<ITreeParser> _parserMock;
        private Mock<ITemplateFactoryCompiler> _compilerMock;
        private Mock<IDocumentWalker> _templateClassBuilderMock;

        [SetUp]
        public void SetUp()
        {
            _parserMock = new Mock<ITreeParser>();
            _compilerMock = new Mock<ITemplateFactoryCompiler>();
            _templateClassBuilderMock = new Mock<IDocumentWalker>();
        }

        [Test]
        public void CompileTemplateFactory_CallsTreeParser()
        {
            // Arrange
            var fakeHamlSource = ViewSourceBuilder.Create();

            // Act
            var compiledTemplate = new CompiledTemplate(_parserMock.Object,
                _templateClassBuilderMock.Object, _compilerMock.Object);
            compiledTemplate.CompileTemplateFactory(fakeHamlSource);

            // Assert
            _parserMock.Verify(x => x.ParseDocument(It.Is<ViewSourceList>(param => param.Contains(fakeHamlSource))));
        }

        [Test]
        public void CompileTemplateFactory_CallsTemplateClassBuilder()
        {
            // Arrange
            var fakeHamlDocument = new HamlDocument();
            _parserMock.Setup(x => x.ParseDocument(It.IsAny<IViewSourceList>()))
                .Returns(fakeHamlDocument);
            var viewSource = ViewSourceBuilder.Create();

            // Act
            var compiledTemplate = new CompiledTemplate(_parserMock.Object,
                _templateClassBuilderMock.Object, _compilerMock.Object);
            compiledTemplate.CompileTemplateFactory(viewSource);

            // Assert
            _templateClassBuilderMock.Verify(x => x.Walk(fakeHamlDocument, It.IsAny<string>()));
        }

        [Test]
        public void CompileTemplateFactory_CallsCompile()
        {
            // Arrange
            var fakeTemplateSource = "FakeTemplateSource";
            _templateClassBuilderMock.Setup(x => x.Walk(It.IsAny<HamlDocument>(), It.IsAny<string>()))
                .Returns(fakeTemplateSource);

            // Act
            var compiledTemplate = new CompiledTemplate(_parserMock.Object,
                _templateClassBuilderMock.Object, _compilerMock.Object);
            compiledTemplate.CompileTemplateFactory(ViewSourceBuilder.Create());

            // Assert
            _compilerMock.Verify(x => x.Compile(fakeTemplateSource));
        }
    }
}