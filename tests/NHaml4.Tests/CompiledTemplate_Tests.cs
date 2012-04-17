using NUnit.Framework;
using NHaml4.Parser;
using Moq;
using NHaml.Tests.Builders;
using NHaml4.Compilers;
using NHaml4.TemplateResolution;
using System.Collections.Generic;
using NHaml4.Walkers;
using System;
using NHaml4.TemplateBase;
using NHaml4.Tests.Builders;

namespace NHaml4.Tests
{
    [TestFixture]
    public class CompiledTemplate_Tests
    {
        private Mock<ITreeParser> _parserMock;
        private Mock<ITemplateFactoryCompiler> _compilerMock;
        private Mock<IDocumentWalker> _documentWalkerMock;

        [SetUp]
        public void SetUp()
        {
            _parserMock = new Mock<ITreeParser>();
            _parserMock.Setup(x => x.ParseViewSource(It.IsAny<IViewSource>()))
                .Returns(HamlDocumentBuilder.Create());
            _compilerMock = new Mock<ITemplateFactoryCompiler>();
            _documentWalkerMock = new Mock<IDocumentWalker>();
        }

        [Test]
        public void CompileTemplateFactory_CallsTreeParser()
        {
            // Arrange
            var fakeHamlSource = ViewSourceBuilder.Create();

            // Act
            var compiledTemplate = new TemplateFactoryFactory(_parserMock.Object,
                _documentWalkerMock.Object, _compilerMock.Object, new List<string>(), new List<string>());
            compiledTemplate.CompileTemplateFactory("className", fakeHamlSource);

            // Assert
            _parserMock.Verify(x => x.ParseViewSource(fakeHamlSource));
        }

        [Test]
        public void CompileTemplateFactory_CallsDocumentWalker()
        {
            // Arrange
            const string className = "className";
            var baseType = typeof(Template);

            var fakeHamlDocument = HamlDocumentBuilder.Create("");
            _parserMock.Setup(x => x.ParseViewSource(It.IsAny<IViewSource>()))
                .Returns(fakeHamlDocument);
            var viewSource = ViewSourceBuilder.Create();
            var imports = new List<string>();

            // Act
            var compiledTemplate = new TemplateFactoryFactory(_parserMock.Object,
                _documentWalkerMock.Object, _compilerMock.Object, new List<string>(), imports);
            compiledTemplate.CompileTemplateFactory(className, viewSource, baseType);

            // Assert
            _documentWalkerMock.Verify(x => x.Walk(fakeHamlDocument, className, baseType, imports));
        }

        [Test]
        public void CompileTemplateFactory_CallsCompile()
        {
            // Arrange
            var fakeTemplateSource = "FakeTemplateSource";
            _documentWalkerMock.Setup(x => x.Walk(It.IsAny<HamlDocument>(), It.IsAny<string>(),
                It.IsAny<Type>(), It.IsAny<IList<string>>()))
                .Returns(fakeTemplateSource);
            var viewSource = ViewSourceBuilder.Create();
            var assemblies = new List<string>();

            // Act
            var compiledTemplate = new TemplateFactoryFactory(_parserMock.Object,
                _documentWalkerMock.Object, _compilerMock.Object, new List<string>(), assemblies);
            compiledTemplate.CompileTemplateFactory(viewSource.GetClassName(), viewSource);

            // Assert
            _compilerMock.Verify(x => x.Compile(fakeTemplateSource, viewSource.GetClassName(), assemblies));
        }
    }
}