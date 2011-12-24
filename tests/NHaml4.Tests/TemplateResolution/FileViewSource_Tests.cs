using NUnit.Framework;
using NHaml4.TemplateResolution;
using System;
using System.IO;

namespace NHaml4.Tests.TemplateResolution
{
    [TestFixture]
    public class FileViewSource_Tests
    {
        [Test]
        public void FileViewSource_NullFileInfo_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FileViewSource(null));
        }

        [Test]
        public void FileViewSource_NonExistingFile_ThrowsFileNotFoundException()
        {
            var fileInfo = new FileInfo(@"c:\non_existing_file.haml");
            Assert.Throws<FileNotFoundException>(() => new FileViewSource(fileInfo));
        }

        [Test]
        public void GetStreamReader_ExistingFile_ReturnsStream()
        {
            var fileInfo = new FileInfo("test.haml");
            var stream = new FileViewSource(fileInfo).GetStreamReader();
            Assert.That(stream.EndOfStream == false);
        }

        [Test]
        public void Path_ExistingFile_ReturnsFullPath()
        {
            const string fileName = "test.haml";
            var fileInfo = new FileInfo(fileName);
            string path = new FileViewSource(fileInfo).Path;

            Assert.That(path.Length, Is.GreaterThan(fileName.Length));
            Assert.That(path, Is.StringContaining(fileName));
        }
    }
}
