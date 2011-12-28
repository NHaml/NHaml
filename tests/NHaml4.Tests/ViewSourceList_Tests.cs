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
    }   
}
