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
        private SimpleTemplateCache _templateCache;
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
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<IViewSource>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource);

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
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<IViewSource>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource, typeof(DummyTemplate));

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }

        [Test]
        public void GetCompiledTemplate_NormalUse_CallsTemplateFactoryFactoryCompileTemplateFactory()
        {
            // Arrange
            var viewSource = ViewSourceBuilder.Create();

            // Act
            _templateEngine.GetCompiledTemplate(viewSource, typeof(DummyTemplate));

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSource.GetClassName(), viewSource));
        }

        [Test]
        public void GetCompiledTemplate_MultipleCalls_OnlyCompilesTemplateOnce()
        {
            // Arrange
            var viewSource = ViewSourceBuilder.Create();

            // Act
            _templateEngine.GetCompiledTemplate(viewSource, typeof(DummyTemplate));
            _templateEngine.GetCompiledTemplate(viewSource, typeof(DummyTemplate));

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSource.GetClassName(), viewSource), Times.Once());
        }

        public void GetCompiledTemplate_NullViewSourceList_ThrowsArgumentNullException()
        {
            // Act
            Assert.Throws<ArgumentNullException>(
                () => _templateEngine.GetCompiledTemplate(null, typeof(DummyTemplate)));
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
        public void GetCompiledTemplateITemplateContentProvider_NullContentProvider_ThrowsArgumentNullException()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => _templateEngine.GetCompiledTemplate(null, "", typeof(DummyTemplate)));
        }

        [Test]
        public void GetCompiledTemplateITemplateContentProvider_NormalUse_CallsGetViewSource()
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
        public void GetCompiledTemplateITemplateContentProvider_NormalUse_ReturnsCorrectTemplateFactory()
        {
            // Arrange
            const string templatePath = "test.haml";
            var contentProviderMock = new Mock<ITemplateContentProvider>();
            contentProviderMock.Setup(x => x.GetViewSource(It.IsAny<string>())).Returns(ViewSourceBuilder.Create());

            var expectedTemplateFactory = new TemplateFactory(typeof(DummyTemplate));
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<IViewSource>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(contentProviderMock.Object, templatePath, typeof(DummyTemplate));

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }
        #endregion

        #region HamlCacheProvider Tests
        [Test]
        public void GetCompiledTemplateIViewSource_MockHamlCacheProvider_CreatesTemplateOnce()
        {
            // Arrange
            var viewSource = ViewSourceBuilder.Create();
            var expectedTemplateFactory = new TemplateFactory(typeof(DummyTemplate));
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<string>(), It.IsAny<IViewSource>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource);

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }
        #endregion
    }
}
