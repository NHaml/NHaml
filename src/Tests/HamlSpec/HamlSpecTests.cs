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
        private readonly FileInfo testFile = new FileInfo("test.haml");

        [Test]
        public void ExecuteTestSuite()
        {
            var hamlSpecTests = GetTheHamlSpecTests();

            foreach (var test in hamlSpecTests)
                ExecuteSingleTest(test);
        }

        private IEnumerable<dynamic> GetTheHamlSpecTests()
        {
            var hamlSpecTests = new List<dynamic>();

            foreach (var group in GetTheTestGroups())
                foreach (var test in GetDictionary(group.Value))
                    hamlSpecTests.Add(GetTheTestToRun(group, test));

            return hamlSpecTests
                .Where(x => x.IgnoreTestsThatHaveAConfigBlock_JustForNow == false);
        }

        private dynamic GetTheTestToRun(KeyValuePair<string, object> group, KeyValuePair<string, object> test)
        {
            dynamic testToRun = new ExpandoObject();

            testToRun.GroupName = group.Key;
            testToRun.TestName = test.Key;

            var properties = GetDictionary(test.Value);
            testToRun.Haml = properties["haml"];
            testToRun.Html = properties["html"];
            testToRun.IgnoreTestsThatHaveAConfigBlock_JustForNow = properties.ContainsKey("config");
            return testToRun;
        }

        private IDictionary<string, object> GetTheTestGroups()
        {
            var json = File.ReadAllText(@"HamlSpec\tests.json");

            var serializer = new JavaScriptSerializer {MaxJsonLength = int.MaxValue};
            var deserializedObject = serializer.DeserializeObject(json);
            return GetDictionary(deserializedObject);
        }

        private static IDictionary<string, object> GetDictionary(object deserializedObject)
        {
            return (IDictionary<string, object>) deserializedObject;
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
            Assert.AreEqual(test.Html, output.ToString(), message);
        }
    }
}