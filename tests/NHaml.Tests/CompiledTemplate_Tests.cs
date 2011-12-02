using NUnit.Framework;
using NHaml.Parser;
using Moq;
using NHaml.Tests.Builders;
using NHaml.Compilers;
using NHaml.TemplateResolution;
using System.Collections.Generic;
using NHaml4;
using NHaml4.Walkers;

namespace NHaml.Tests
{
    [TestFixture]
    public class CompiledTemplate_Tests
    {
        private Mock<ITreeParser> _parserMock;
        private Mock<ITemplateFactoryCompiler> _compilerMock;
        private Mock<IWalker> _templateClassBuilderMock;

        [SetUp]
        public void SetUp()
        {
            _parserMock = new Mock<ITreeParser>();
            _compilerMock = new Mock<ITemplateFactoryCompiler>();
            _templateClassBuilderMock = new Mock<IWalker>();
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
            _parserMock.Verify(x => x.ParseDocument(It.Is<IList<IViewSource>>(y => y.Contains(fakeHamlSource))));
        }

        [Test]
        public void CompileTemplateFactory_CallsTemplateClassBuilder()
        {
            // Arrange
            var fakeHamlDocument = new HamlDocument();
            _parserMock.Setup(x => x.ParseDocument(It.IsAny<IList<IViewSource>>()))
                .Returns(fakeHamlDocument);

            // Act
            var compiledTemplate = new CompiledTemplate(_parserMock.Object,
                _templateClassBuilderMock.Object, _compilerMock.Object);
            compiledTemplate.CompileTemplateFactory(ViewSourceBuilder.Create());

            // Assert
            _templateClassBuilderMock.Verify(x => x.ParseHamlDocument(fakeHamlDocument));
        }

        [Test]
        public void CompileTemplateFactory_CallsCompile()
        {
            // Arrange
            var fakeTemplateSource = "FakeTemplateSource";
            _templateClassBuilderMock.Setup(x => x.ParseHamlDocument(It.IsAny<HamlDocument>()))
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