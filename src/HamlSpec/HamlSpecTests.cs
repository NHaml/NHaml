using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using NUnit.Framework;
using System;
using NHaml;
using NHaml.Compilers.CSharp2;
using System.Diagnostics;

namespace HamlSpec
{
    [TestFixture]
    public class HamlSpecTests
    {
        private string TemplatesFolder = @"Functional\Templates\";

        // WORKING
        //[TestCase("silent comments")]
        //[TestCase("markup comments")]
        //[TestCase("tags with unusual HTML characters")]
        //[TestCase("tags with unusual CSS identifiers")]
        // TODO
        //[TestCase("headers")]
        [TestCase("basic Haml tags and CSS")]
        //[TestCase("tags with inline content")]
        //[TestCase("tags with nested content")]
        //[TestCase("tags with HTML-style attributes")]
        //[TestCase("tags with Ruby-style attributes")]
        //[TestCase("internal filters")]
        //[TestCase("Ruby-style interpolation")]
        //[TestCase("HTML escaping")]
        //[TestCase("boolean attributes")]
        //[TestCase("whitespace preservation")]
        //[TestCase("whitespace removal")]
        public void ExecuteTestSuite(string groupName)
        {
            var hamlSpecTests = new HamlSpecLoader().GetTheHamlSpecTests(groupName);

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

        private void ExecuteSingleTest(HamlSpec test)
        {
            var template = CreateTemplate(test.Haml);
            var output = new StringWriter();
            template.Render(output);

            //TODO - Get this crappy reformatting fix out!
            string expected = test.ExpectedHtml.Replace("\n", "").Replace("\r", "");
            string actual = output.ToString().Replace("\n", "").Replace("\r", "");

            var message = string.Format("{0} - {1}", test.GroupName, test.TestName);
            Assert.AreEqual(expected, actual, message);
        }

        private Template CreateTemplate(string hamlTemplate)
        {
            FileInfo testFile = new FileInfo("test.haml");
            if (testFile.Exists) testFile.Delete();
            File.WriteAllText(testFile.FullName, hamlTemplate);

            var templateEngine = GetTemplateEngine();
            var resources = new TemplateCompileResources(templateEngine.Options.TemplateBaseType, testFile.FullName);
            var compiledTemplate = templateEngine.Compile(resources);
            return compiledTemplate.CreateInstance();
        }

        private TemplateEngine GetTemplateEngine()
        {
            var templateOptions = new TemplateOptions
            {
                TemplateCompiler = new CSharp2TemplateCompiler(),
            };
            templateOptions.TemplateContentProvider.AddPathSource(TemplatesFolder);
            return new TemplateEngine(templateOptions);
        }
    }
}