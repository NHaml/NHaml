using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using NUnit.Framework;
using System;

namespace HamlSpec
{
    [TestFixture]
    public class HamlSpecTests : TestFixtureBase
    {
        private readonly FileInfo testFile = new FileInfo("test.haml");


        [TestCase("headers")]
        [TestCase("basic Haml tags and CSS")]
        [TestCase("tags with unusual HTML characters")]
        [TestCase("tags with unusual CSS identifiers")]
        [TestCase("tags with inline content")]
        [TestCase("tags with nested content")]
        [TestCase("tags with HTML-style attributes")]
        [TestCase("tags with Ruby-style attributes")]
        [TestCase("silent comments")]
        [TestCase("markup comments")]
        [TestCase("internal filters")]
        [TestCase("Ruby-style interpolation")]
        [TestCase("HTML escaping")]
        [TestCase("boolean attributes")]
        [TestCase("whitespace preservation")]
        [TestCase("whitespace removal")]
        public void ExecuteTestSuite(string groupName)
        {
            var hamlSpecTests = GetTheHamlSpecTests(groupName);

            int errorCount = 0;
            foreach (var test in hamlSpecTests)
            {
                string testName = test.TestName;

                try
                {
                    ExecuteSingleTest(test);
                    Console.WriteLine("PASS : " + testName);
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Console.WriteLine("FAIL - " + testName);
                    Console.WriteLine(ex.Message);
                }
            }
            Assert.AreEqual(0, errorCount, errorCount + " of " + hamlSpecTests.Count() + " scenarios failed!");

        }

        private IEnumerable<dynamic> GetTheHamlSpecTests(string groupName)
        {
            var hamlSpecTests = new List<dynamic>();
            var group = GetTestGroup(groupName);
                foreach (var test in GetDictionary(group))
                    hamlSpecTests.Add(GetTheTestToRun(groupName, test));

            return hamlSpecTests
                .Where(x => x.IgnoreTestsThatHaveAConfigBlock_JustForNow == false);
        }

        private object GetTestGroup(string groupName)
        {
            var json = File.ReadAllText(@"tests.json");

            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var deserializedObject = serializer.DeserializeObject(json);
            return ((IDictionary<string, object>)deserializedObject)[groupName];
        }

        private dynamic GetTheTestToRun(string groupName, KeyValuePair<string, object> test)
        {
            dynamic testToRun = new ExpandoObject();

            testToRun.GroupName = groupName;
            testToRun.TestName = test.Key;

            var properties = GetDictionary(test.Value);
            testToRun.Haml = properties["haml"];
            testToRun.Html = properties["html"];
            testToRun.IgnoreTestsThatHaveAConfigBlock_JustForNow = properties.ContainsKey("config");
            return testToRun;
        }

        private static IDictionary<string, object> GetDictionary(object deserializedObject)
        {
            return (IDictionary<string, object>)deserializedObject;
        }

        private void ExecuteSingleTest(dynamic test)
        {
            if (testFile.Exists)
                testFile.Delete();

            test.Html = test.Html.Replace("'", "\"") + "\r\n";

            File.WriteAllText(testFile.FullName, test.Haml);

            var template = CreateTemplate(testFile.FullName);

            var output = new StringWriter();
            template.Render(output);

            var message = string.Format("{0} - {1}", test.GroupName, test.TestName);

            //TODO - Get this crappy reformatting fix out!
            test.Html = test.Html.Replace("\n", "").Replace("\r", "");
            string result = output.ToString().Replace("\n", "").Replace("\r", "");

            Assert.AreEqual(test.Html, result, message);
        }
    }
}