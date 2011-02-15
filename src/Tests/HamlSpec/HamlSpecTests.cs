using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using NUnit.Framework;

namespace NHaml.Tests.HamlSpec
{
    [TestFixture]
    public class HamlSpecTests : TestFixtureBase
    {
        private readonly FileInfo _testFile = new FileInfo("test.haml");

        [Test]
        public void ExecuteTestSuite()
        {
            var json = File.ReadAllText(@"HamlSpec\tests.json");

            var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var testGroups = (IDictionary<string, object>)serializer.DeserializeObject(json);

            foreach(var groupName in testGroups.Keys)
            {
                var tests = (IDictionary<string, object>)testGroups[groupName];

                foreach (var testName in tests.Keys)
                {
                    var properties = (IDictionary<string, object>)tests[testName];

                    var haml = (string)properties["haml"];
                    var html = (string)properties["html"];

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