using System.IO;
using NHaml.Conversion.GUI;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class ImporterTests
    {
        [Test]
        public void AutoCloseTest()
        {
            var importer = new Importer {IncludeDocType = false};
            RunTest(importer, "AutoClose");
        }   

        [Test]
        public void FourSpacesTest()
        {
            var importer = new Importer
                               {
                                   IndentString = "    "
                               };
            RunTest(importer, "4Spaces");
        }

        [Test]
        public void ReferenceExample1Test()
        {
            var importer = new Importer { IncludeDocType = false };
            RunTest(importer, "ReferenceExample1");
        }
     
        [Test]
        public void ReferenceExample2Test()
        {
            var importer = new Importer
                               {
                                   IncludeDocType = false,
                               };
            RunTest(importer, "ReferenceExample2");
        }
     
        [Test]
        public void ClassTest()
        {
            var importer = new Importer
                               {
                                   IncludeDocType = false,
                               };
            RunTest(importer, "Class");
        }
        [Test]
        public void IdTest()
        {
            var importer = new Importer
                               {
                                   IncludeDocType = false,
                               };
            RunTest(importer, "Id");
        }
     
        [Test]
        public void TabsTest()
        {
            var importer = new Importer
                               {
                                   IndentString = "\t"
                               };
            RunTest(importer, "Tabs");
        }
     
 
        [Test]
        public void VeryBasicTest()
        {
            var importer = new Importer();
            RunTest(importer, "VeryBasic");
        }


        [Test]
        public void WhitespaceSensitiveTest()
        {
            var importer = new Importer {IncludeDocType = false};
            RunTest(importer, "WhitespaceSensitive");
        }

        private static void RunTest(Importer importer, string file)
        {
            string actual;
            using (var memoryStream = new MemoryStream())
            {
                using (var textWriter = new StreamWriter(memoryStream))
                {
                    using (var reader = File.OpenText("Expected/" + file + ".xhtml"))
                    {
                        importer.Import(reader, textWriter);
                    }

                    memoryStream.Position = 0;
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        actual = streamReader.ReadToEnd();
                    } 
                }
            }
            var expectedText = File.ReadAllText("Templates/" + file + ".haml");
            Assert.AreEqual(expectedText, actual);
        }
    }
}