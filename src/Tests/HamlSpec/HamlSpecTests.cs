using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace NHaml.Tests.HamlSpec
{
    [TestFixture]
    public class HamlSpecTests : TestFixtureBase
    {
        private readonly FileInfo _testFile = new FileInfo("test.haml");

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
            var jsonTests = File.ReadAllText(@"HamlSpec\tests.json");
            var testGroups = (Hashtable)JSON.JsonDecode(jsonTests);

            foreach(DictionaryEntry testGroup in testGroups)
            {
                int errorCount = 0;

                if ((string)testGroup.Key != groupName) continue;
                var tests = (Hashtable)testGroup.Value;

                foreach(DictionaryEntry test in tests)
                {
                    var testName = (string)test.Key;

                    var testValues = (Hashtable)test.Value;

                    var haml = (string)testValues["haml"];
                    var html = (string)testValues["html"];

                    try
                    {
                        ExecuteSingleTest(groupName, testName, haml, html);
                        Console.WriteLine("PASS : " + testName);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        Console.WriteLine("FAIL - " + testName);
                        Console.WriteLine(ex.Message);
                    }
                }
                Assert.AreEqual(0, errorCount, errorCount + " of " + tests.Count + " scenarios failed!");
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

            //TODO - Get this crappy reformatting fix out!
            html = html.Replace("\n", "").Replace("\r", "");
            string result = output.ToString().Replace("\n", "").Replace("\r", "");

            Assert.AreEqual(html, result, message);
        }
    }
}