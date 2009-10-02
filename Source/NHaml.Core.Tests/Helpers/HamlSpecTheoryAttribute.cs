using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Extensions;
using Xunit.Sdk;

namespace NHaml.Core.Tests.Helpers
{
    public class HamlSpecTheoryAttribute : FactAttribute
    {
        private string _jsonTests;

        protected override IEnumerable<ITestCommand> EnumerateTestCommands(IMethodInfo method)
        {
            var testType = method.MethodInfo.DeclaringType;
            var resourceName = testType.Namespace + ".tests.json";
            var resourceStream = testType.Assembly.GetManifestResourceStream(resourceName);

            if(resourceStream == null)
                throw new FileNotFoundException("resource " + resourceName + " not found");

            using(var reader = new StreamReader(resourceStream, Encoding.UTF8, false))
                _jsonTests = reader.ReadToEnd();

            var testGroups = (Hashtable)JSON.JsonDecode(_jsonTests);

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

                    var config = (Hashtable)testValues["config"];

                    var format = config != null ? config["format"] : null;

                    html = html.Replace("\n", System.Environment.NewLine);

                    var fullName = string.Format("{1} ({0})", groupName, testName);
                    yield return new TheoryCommand(method, fullName, new[] { fullName, haml, html, format });
                }
            }
        }
    }
}