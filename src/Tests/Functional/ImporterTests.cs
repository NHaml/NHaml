using System.IO;
using NUnit.Framework;

namespace NHaml.Tests.Functional
{
    [TestFixture]
    public class ImporterTests
    {
        [Test]
        public void AutoCloseTest()
        {
            var importer = new Generator.Wpf.Generator {IncludeDocType = false};
            RunTest(importer, "AutoClose");
        }   

        [Test]
        public void FourSpacesTest()
        {
            var importer = new Generator.Wpf.Generator
            {
                IndentString = "    "
            };
            RunTest(importer, "4Spaces");
        }

        [Test]
        public void ReferenceExample1Test()
        {
            var importer = new Generator.Wpf.Generator { IncludeDocType = false };
            RunTest(importer, "ReferenceExample1");
        }
     
        [Test]
        public void ReferenceExample2Test()
        {
            var importer = new Generator.Wpf.Generator
            {
                IncludeDocType = false,
            };
            RunTest(importer, "ReferenceExample2");
        }
     
        [Test]
        public void ClassTest()
        {
            var importer = new Generator.Wpf.Generator
            {
                IncludeDocType = false,
            };
            RunTest(importer, "Class");
        }
        [Test]
        public void IdTest()
        {
            var importer = new Generator.Wpf.Generator
            {
                IncludeDocType = false,
            };
            RunTest(importer, "Id");
        }
     
        [Test]
        public void TabsTest()
        {
            var importer = new Generator.Wpf.Generator
            {
                IndentString = "\t"
            };
            RunTest(importer, "Tabs");
        }
     
 
        [Test]
        public void VeryBasicTest()
        {
            var importer = new Generator.Wpf.Generator();
            RunTest(importer, "VeryBasic");
        }


        [Test]
        public void WhitespaceSensitiveTest()
        {
            var importer = new Generator.Wpf.Generator {IncludeDocType = false};
            RunTest(importer, "WhitespaceSensitive");
        }

        private static void RunTest(Generator.Wpf.Generator generator, string file)
        {
            string actual;
            using (var memoryStream = new MemoryStream())
            {
                using (var textWriter = new StreamWriter(memoryStream))
                {
                    using (var reader = File.OpenText(string.Format("Functional/Expected/{0}.xhtml", file)))
                    {
                        generator.Import(reader, textWriter);
                    }

                    memoryStream.Position = 0;
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        actual = streamReader.ReadToEnd();
                    } 
                }
            }
            var expectedText = File.ReadAllText(string.Format("Functional/Templates/{0}.haml", file));
            Assert.AreEqual(expectedText, actual);
        }
    }
}