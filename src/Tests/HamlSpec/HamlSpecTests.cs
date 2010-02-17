using System.Collections;
using System.IO;
using NUnit.Framework;

namespace NHaml.Tests.HamlSpec
{
    [TestFixture]
    public class HamlSpecTests : TestFixtureBase
    {
        private readonly FileInfo _testFile = new FileInfo("test.haml");

        [Test,Ignore]
        public void ExecuteTestSuite()
        {
            var jsonTests = File.ReadAllText(@"HamlSpec\tests.json");
            var testGroups = (Hashtable)JSON.JsonDecode(jsonTests);

            foreach(DictionaryEntry testGroup in testGroups)
            {
                var groupName = (string)testGroup.Key;
                var tests = (Hashtable)testGroup.Value;

                foreach(DictionaryEntry test in tests)
                {
                    var testName = (string)test.Key;
                    var testValues = (Hashtable)test.Value;

                    var haml = (string)testValues["haml"];
                    var html = (string)testValues["html"];

                    ExecuteSingleTest(groupName, testName, haml, html);
                }
            }
        }

        private void ExecuteSingleTest(string groupName, string testName, string haml, string html)
        {
            if(_testFile.Exists)
                _testFile.Delete();

            html = html.Replace("'", "\"");

            File.WriteAllText(_testFile.FullName,haml);

            var template = CreateTemplate(_testFile.FullName);

            var output = new StringWriter();
            template.Render(output);

            var message = string.Format("{0} - {1}",groupName,testName);
            Assert.AreEqual(html, output.ToString(),message);
        }
    }
}