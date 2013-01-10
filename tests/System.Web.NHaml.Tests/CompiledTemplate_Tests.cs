using System.Web.NHaml;
using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Web.NHaml.TemplateBase;
using System.Web.NHaml.TemplateResolution;
using System.Web.NHaml.Walkers;
using NUnit.Framework;
using Moq;
using NHaml.Tests.Builders;
using System.Collections.Generic;
using System;

namespace NHaml.Tests
{
    [TestFixture]
    public class CompiledTemplate_Tests
    {
        private Mock<ITreeParser> _parserMock;
        private Mock<ITemplateFactoryCompiler> _compilerMock;
        private Mock<IDocumentWalker> _documentWalkerMock;
        private Mock<ITemplateContentProvider> _templateContentProviderMock;
        private Mock<ITemplateContentProvider> _contentProviderMock;

        [SetUp]
        public void SetUp()
        {
            _parserMock = new Mock<ITreeParser>();
            _parserMock.Setup(x => x.ParseViewSource(It.IsAny<ViewSource>()))
                .Returns(HamlDocumentBuilder.Create());
            _compilerMock = new Mock<ITemplateFactoryCompiler>();
            _documentWalkerMock = new Mock<IDocumentWalker>();
            _templateContentProviderMock = new Mock<ITemplateContentProvider>();
            _contentProviderMock = new Mock<ITemplateContentProvider>();
        }

        [Test]
        public void CompileTemplateFactory_CallsTreeParser()
        {
            // Arrange
            var fakeHamlSource = ViewSourceBuilder.Create();

            // Act
            var compiledTemplate = new TemplateFactoryFactory(_templateContentProviderMock.Object, _parserMock.Object,
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
            _parserMock.Setup(x => x.ParseViewSource(It.IsAny<ViewSource>()))
                .Returns(fakeHamlDocument);
            var viewSource = ViewSourceBuilder.Create();
            var imports = new List<string>();

            // Act
            var compiledTemplate = new TemplateFactoryFactory(_contentProviderMock.Object, _parserMock.Object,
                _documentWalkerMock.Object, _compilerMock.Object, new List<string>(), imports);
            compiledTemplate.CompileTemplateFactory(className, new ViewSourceCollection { viewSource }, baseType);

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
            var compiledTemplate = new TemplateFactoryFactory(_contentProviderMock.Object, _parserMock.Object,
                _documentWalkerMock.Object, _compilerMock.Object, new List<string>(), assemblies);
            compiledTemplate.CompileTemplateFactory(viewSource.GetClassName(), viewSource);

            // Assert
            _compilerMock.Verify(x => x.Compile(fakeTemplateSource, viewSource.GetClassName(), assemblies));
        }
    }
}