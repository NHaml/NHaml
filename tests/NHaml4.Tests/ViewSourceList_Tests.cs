using System.IO;
using Moq;
using NHaml4.TemplateResolution;
using NHaml4.Tests.Mocks;
using NUnit.Framework;

namespace NHaml4.Tests
{
    [TestFixture]
    public class ViewSourceList_Tests
    {
        [Test]
        public void Constructor_WithFileInfoArgument_AddsToCollection()
        {
            var list = new ViewSourceList(new FileInfo("test.haml"));
            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void GetClassNameFromPathName_ValidPath_ReturnsValidClass()
        {
            // Arrange
            var list = new ViewSourceList { ViewSourceWithPath("test.haml").Object };
            const string expectedClassName = "test_haml";

            // Act
            string actual = list.GetClassNameFromPathName();

            // Assert
            Assert.AreEqual(expectedClassName, actual);
        }

        [Test]
        public void GetClassNameFromPathName_MultiplePaths_ReturnsValidClass()
        {
            // Arrange
            var list = new ViewSourceList
                           {
                               ViewSourceWithPath("A.haml").Object,
                               ViewSourceWithPath("B.haml").Object
                           };
            const string expectedClassName = "B_haml";

            // Act
            string actual = list.GetClassNameFromPathName();

            // Assert
            Assert.AreEqual(expectedClassName, actual);
        }

        [Test]
        public void GetCacheKey_MultiplePaths_ReturnsCorrectKey()
        {
            // Arrange
            var list = new ViewSourceList
                           {
                               ViewSourceWithPath("A.haml").Object,
                               ViewSourceWithPath("B.haml").Object
                           };
            const string expectedClassName = "A.haml,B.haml";

            // Act
            string actual = list.GetCacheKey();

            // Assert
            Assert.AreEqual(expectedClassName, actual);
        }



        private static Mock<IViewSource> ViewSourceWithPath(string pathName1)
        {
            var viewSourceMock1 = new Mock<IViewSource>();
            viewSourceMock1.SetupGet(x => x.Path).Returns(pathName1);
            return viewSourceMock1;
        }
    }   
}
