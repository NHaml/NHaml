using System;
using NHaml.Tests.Builders;
using NHaml4.TemplateBase;
using NHaml4.TemplateResolution;
using NUnit.Framework;
using Moq;

namespace NHaml4.Tests
{
    [TestFixture]
    public class TemplateEngine_Tests
    {
        private Mock<ITemplateFactoryFactory> _templateFactoryFactoryMock;
        private IHamlTemplateCache _templateCache;
        private TemplateEngine _templateEngine;

        class DummyTemplate : Template
        { }

        [SetUp]
        public void SetUp()
        {
            _templateCache = new SimpleTemplateCache();
            _templateCache.Clear();
            _templateFactoryFactoryMock = new Mock<ITemplateFactoryFactory>();
            _templateEngine = new TemplateEngine(_templateCache, _templateFactoryFactoryMock.Object);
        }

        #region GetCompiledTemplate(IViewSource) Tests
        [Test]
        public void GetCompiledTemplateIViewSource_NormalUse_ReturnsCompiledTemplateFactory()
        {
            // Arrange
            var viewSource = ViewSourceBuilder.Create();
            var expectedTemplateFactory = new TemplateFactory(typeof(DummyTemplate));
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<ViewSourceCollection>(), It.IsAny<Type>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource, typeof(object));

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }
        #endregion

        #region GetCompiledTemplate(IViewSource, Type) Tests
        [Test]
        public void GetCompiledTemplate_NormalUse_ReturnsCompiledTemplateFactory()
        {
            // Arrange
            var viewSource = ViewSourceBuilder.Create();
            var expectedTemplateFactory = new TemplateFactory(typeof(DummyTemplate));
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<ViewSourceCollection>(), It.IsAny<Type>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource, typeof(object));

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }

        [Test]
        public void GetCompiledTemplate_NormalUse_CallsTemplateFactoryFactoryCompileTemplateFactory()
        {
            // Arrange
            var viewSourceCollection = new ViewSourceCollection { ViewSourceBuilder.Create() };

            // Act
            _templateEngine.GetCompiledTemplate(viewSourceCollection, typeof(object));

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSourceCollection.GetClassName(), viewSourceCollection, It.IsAny<Type>()));
        }

        [Test]
        public void GetCompiledTemplate_NormalUse_UsesDefaultTemplateBaseType()
        {
            // Arrange
            var viewSourceCollection = new ViewSourceCollection { ViewSourceBuilder.Create() };

            // Act
            _templateEngine.GetCompiledTemplate(viewSourceCollection, typeof(object));

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSourceCollection.GetClassName(), viewSourceCollection, typeof(object)));
        }

        [Test]
        public void GetCompiledTemplate_BaseTemplateSpecified_UsesSpecifiedTemplateBaseType()
        {
            // Arrange
            var viewSourceCollection = new ViewSourceCollection { ViewSourceBuilder.Create() };

            // Act
            _templateEngine.GetCompiledTemplate(viewSourceCollection, typeof(object));

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSourceCollection.GetClassName(), viewSourceCollection, typeof(object)));
        }
        
        [Test]
        public void GetCompiledTemplate_MultipleCalls_OnlyCompilesTemplateOnce()
        {
            // Arrange
            var viewSourceCollection = new ViewSourceCollection { ViewSourceBuilder.Create() };

            // Act
            _templateEngine.GetCompiledTemplate(viewSourceCollection, typeof(DummyTemplate));
            _templateEngine.GetCompiledTemplate(viewSourceCollection, typeof(DummyTemplate));

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSourceCollection.GetClassName(), viewSourceCollection, typeof(DummyTemplate)), Times.Once());
        }

        public void GetCompiledTemplate_NullViewSourceList_ThrowsArgumentNullException()
        {
            // Act
            Assert.Throws<ArgumentNullException>(
                () => _templateEngine.GetCompiledTemplate((IViewSource)null, typeof(DummyTemplate)));
        }

        [Test]
        public void GetCompiledTemplate_NullBaseType_ThrowsArgumentNullException()
        {
            // Act
            Assert.Throws<ArgumentNullException>(
                () => _templateEngine.GetCompiledTemplate(ViewSourceBuilder.Create(), null));
        }
        #endregion

        #region GetCompiledTemplate(ITemplateContentProvider, string, Type) Tests
        [Test]
        public void GetCompiledTemplate_NullContentProvider_ThrowsArgumentNullException()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => _templateEngine.GetCompiledTemplate(null, "", typeof(DummyTemplate)));
        }

        [Test]
        public void GetCompiledTemplate_NormalUse_CallsGetViewSource()
        {
            // Arrange
            const string templatePath = "test.haml";
            var contentProviderMock = new Mock<ITemplateContentProvider>();
            contentProviderMock.Setup(x => x.GetViewSource(It.IsAny<string>())).Returns(ViewSourceBuilder.Create());

            // Act
            _templateEngine.GetCompiledTemplate(contentProviderMock.Object, templatePath, typeof(DummyTemplate));

            // Assert
            contentProviderMock.Verify(x => x.GetViewSource(templatePath));
        }

        [Test]
        public void GetCompiledTemplate_NormalUse_ReturnsCorrectTemplateFactory()
        {
            // Arrange
            const string templatePath = "test.haml";
            var contentProviderMock = new Mock<ITemplateContentProvider>();
            contentProviderMock.Setup(x => x.GetViewSource(It.IsAny<string>())).Returns(ViewSourceBuilder.Create());

            var expectedTemplateFactory = new TemplateFactory(typeof(DummyTemplate));
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<ViewSourceCollection>(), typeof(DummyTemplate)))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(contentProviderMock.Object, templatePath, typeof(DummyTemplate));

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }
        #endregion

        #region HamlCacheProvider Tests
        [Test]
        public void GetCompiledTemplate_MockHamlCacheProvider_CreatesTemplateOnce()
        {
            // Arrange
            var viewSource = ViewSourceBuilder.Create();
            var expectedTemplateFactory = new TemplateFactory(typeof(DummyTemplate));
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<ViewSourceCollection>(), It.IsAny<Type>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource, typeof(object));

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }
        #endregion
    }
}
