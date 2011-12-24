using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;

namespace NHaml4.Tests
{
    [TestFixture]
    public class TemplateEngine_Tests
    {
        private Mock<ITemplateFactoryFactory> _templateFactoryFactoryMock;
        private TemplateEngine _templateEngine;

        class DummyTemplate : TemplateBase.Template
        { }

        [SetUp]
        public void SetUp()
        {
            _templateFactoryFactoryMock = new Mock<ITemplateFactoryFactory>();
            _templateEngine = new TemplateEngine(_templateFactoryFactoryMock.Object);
        }

        [Test]
        public void GetCompiledTemplate_NormalUse_ReturnsCompiledTemplateFactory()
        {
            // Arrange
            var viewSourceList = new ViewSourceList(new System.IO.FileInfo("test.haml"));
            var expectedTemplateFactory = new TemplateFactory(typeof(DummyTemplate));
            _templateFactoryFactoryMock.Setup(x => x.CompileTemplateFactory(It.IsAny<ViewSourceList>()))
                .Returns(expectedTemplateFactory);

            // Act
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSourceList);

            // Assert
            Assert.That(templateFactory, Is.SameAs(expectedTemplateFactory));
        }

        [Test]
        public void GetCompiledTemplate_NormalUse_CallsTemplateFactoryFactoryCompileTemplateFactory()
        {
            // Arrange
            var viewSourceList = new ViewSourceList(new System.IO.FileInfo("test.haml"));

            // Act
            _templateEngine.GetCompiledTemplate(viewSourceList);

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSourceList));
        }

        [Test]
        public void GetCompiledTemplate_MultipleCalls_OnlyCompilesTemplateOnce()
        {
            // Arrange
            var viewSourceList = new ViewSourceList(new System.IO.FileInfo("test.haml"));

            // Act
            _templateEngine.GetCompiledTemplate(viewSourceList);
            _templateEngine.GetCompiledTemplate(viewSourceList);

            // Assert
            _templateFactoryFactoryMock.Verify(x => x.CompileTemplateFactory(viewSourceList), Times.Once());
        }

        [Test]
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
                () => _templateEngine.GetCompiledTemplate(new ViewSourceList(new System.IO.FileInfo("test.haml")), null));
        }
    }
}
