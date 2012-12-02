using NUnit.Framework;
using NHaml.Parser;
using NHaml.Parser.Rules;
using NHaml.Walkers;
using NHaml.Compilers;
using Moq;
using System.Collections.Generic;
using NHaml.TemplateResolution;
using NHaml.Tests.Builders;
using System.Linq;
using NHaml.IO;
using NHaml.Tests.Builders;

namespace NHaml.Tests
{
    [TestFixture]
    public class TemplateFactoryFactory_Tests
    {
        private Mock<ITreeParser> _treeParserMock;
        private Mock<IDocumentWalker> _documentWalkerMock;
        private Mock<ITemplateFactoryCompiler> _templateCompilerMock;
        private Mock<ITemplateContentProvider> _templateContentProviderMock;
        private TemplateFactoryFactory _templateFactoryFactory;

        [SetUp]
        public void Setup()
        {
            _treeParserMock = new Mock<ITreeParser>();
            _documentWalkerMock = new Mock<IDocumentWalker>();
            _templateCompilerMock = new Mock<ITemplateFactoryCompiler>();
            _templateContentProviderMock = new Mock<ITemplateContentProvider>();
            _templateFactoryFactory = new TemplateFactoryFactory(_templateContentProviderMock.Object,
                _treeParserMock.Object,
                _documentWalkerMock.Object,
                _templateCompilerMock.Object,
                new List<string>(),
                new List<string>());
        }

        [Test]
        public void BuildHamlDocument_NoPartials_ReturnsSameDocument()
        {
            var viewSource = ViewSourceBuilder.Create("Test");            
            var hamlDocument = HamlDocumentBuilder.Create("MainFile",
                new HamlNodeTextContainer(0, "Test"));
            _treeParserMock.Setup(x => x.ParseViewSource(viewSource))
                .Returns(hamlDocument);

            var viewSourceCollection = new ViewSourceCollection {viewSource};

            var result = _templateFactoryFactory.BuildHamlDocument(viewSourceCollection);

            Assert.That(result, Is.SameAs(hamlDocument));
            Assert.That(result.Children.First(), Is.SameAs(hamlDocument.Children.First()));
        }

        [Test]
        public void BuildHamlDocument_PartialReference_CombinesTwoDocuments()
        {
            var rootViewSource = ViewSourceBuilder.Create("MainFile");
            var rootDocument = HamlDocumentBuilder.Create("MainFile",
                new HamlNodeTextContainer(0, "Test"),
                new HamlNodePartial(new HamlLine("SubDocument", HamlRuleEnum.Partial, "", 0)));
            _treeParserMock.Setup(x => x.ParseViewSource(rootViewSource))
                .Returns(rootDocument);

            var childViewSource = ViewSourceBuilder.Create("SubDocument", "SubDocument");
            var childDocument = HamlDocumentBuilder.Create("SubDocument)",
                new HamlNodeTextContainer(0, "Child Test"));
            _treeParserMock.Setup(x => x.ParseViewSource(childViewSource))
                .Returns(childDocument);

            var viewSourceList = new ViewSourceCollection { rootViewSource, childViewSource };

            var result = _templateFactoryFactory.BuildHamlDocument(viewSourceList);

            Assert.That(result, Is.SameAs(rootDocument));
            Assert.That(((HamlNodePartial)result.Children.ToList()[1]).IsResolved, Is.True);
            Assert.That(result.Children.ToList()[1].Children.First(), Is.SameAs(childDocument.Children.First()));
        }

        [Test]
        public void BuildHamlDocument_UnnamedPartialReference_UsesFollowingPartial()
        {
            var rootViewSource = ViewSourceBuilder.Create("MainFile");
            var rootDocument = HamlDocumentBuilder.Create("MainFile",
                new HamlNodePartial(new HamlLine("", HamlRuleEnum.Partial, "", 0)));
            _treeParserMock.Setup(x => x.ParseViewSource(rootViewSource))
                .Returns(rootDocument);

            var childViewSource = ViewSourceBuilder.Create("SubDocument", "SubDocument");
            var childDocument = HamlDocumentBuilder.Create("SubDocument)",
                new HamlNodeTextContainer(0, "Child Test"));
            _treeParserMock.Setup(x => x.ParseViewSource(childViewSource))
                .Returns(childDocument);

            var viewSourceList = new ViewSourceCollection { rootViewSource, childViewSource };

            var result = _templateFactoryFactory.BuildHamlDocument(viewSourceList);

            Assert.That(result, Is.SameAs(rootDocument));
            Assert.That(((HamlNodePartial)result.Children.First()).IsResolved, Is.True);
            Assert.That(result.Children.First().Children.First(), Is.SameAs(childDocument.Children.First()));
        }
        [Test]
        public void BuildHamlDocument_UnknownPartial_UsesTemplateContentProvider()
        {
            const string partialName = "SubDocument";
            var rootViewSource = ViewSourceBuilder.Create("MainFile");
            var rootDocument = HamlDocumentBuilder.Create("MainFile",
                new HamlNodePartial(new HamlLine(partialName, HamlRuleEnum.Partial, "", 0)));
            _treeParserMock.Setup(x => x.ParseViewSource(rootViewSource))
                .Returns(rootDocument);
            var viewSourceList = new ViewSourceCollection { rootViewSource };

            var childViewSource = ViewSourceBuilder.Create("SubDocument", partialName);
            var childDocument = HamlDocumentBuilder.Create("SubDocument)",
                new HamlNodeTextContainer(0, "Child Test"));
            _treeParserMock.Setup(x => x.ParseViewSource(childViewSource))
                .Returns(childDocument);

            _templateContentProviderMock.Setup(x => x.GetViewSource(partialName))
                .Returns(childViewSource);

            var result = _templateFactoryFactory.BuildHamlDocument(viewSourceList);

            _templateContentProviderMock.Verify(x => x.GetViewSource(partialName));
        }
    }
}
