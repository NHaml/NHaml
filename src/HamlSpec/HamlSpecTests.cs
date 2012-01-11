using System.IO;
using System.Linq;
using NHaml4.TemplateResolution;
using NUnit.Framework;
using System;
using NHaml4;
using NHaml4.TemplateBase;
using NHaml4.Walkers.CodeDom;

namespace HamlSpec
{
    [TestFixture]
    public class HamlSpecTests
    {
        private int _totalNoTests;
        private int _totalNoTestsFailed;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _totalNoTests = 0;
            _totalNoTestsFailed = 0;
        }
        
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Console.WriteLine(string.Format("{0} OF {1} TESTS FAILED.", _totalNoTestsFailed, _totalNoTests));            
        }

        // WORKING
        [TestCase("plain text templates")]
        [TestCase("tags with unusual HTML characters")]
        [TestCase("tags with unusual CSS identifiers")]
        [TestCase("basic Haml tags and CSS")]
        [TestCase("silent comments")]
        [TestCase("tags with inline content")]
        [TestCase("markup comments")]
        [TestCase("tags with nested content")]
        [TestCase("tags with HTML-style attributes")]
        [TestCase("boolean attributes")]

        // IN PROGRESS

        // TODO
        //[TestCase("tags with HTML-style attributes and variables")]
        //[TestCase("headers")]
        //[TestCase("tags with Ruby-style attributes")]
        //[TestCase("internal filters")]
        //[TestCase("Ruby-style interpolation")]
        //[TestCase("HTML escaping")]
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
            int totalCount = hamlSpecTests.Count();
            Console.WriteLine(errorCount + " of " + totalCount + " scenarios failed.");

            _totalNoTests += totalCount;
            _totalNoTestsFailed += errorCount;

            Assert.AreEqual(0, errorCount, errorCount + " of " + totalCount + " scenarios failed.");
        }

        private void ExecuteSingleTest(HamlSpec test)
        {
            var template = CreateTemplate(test.Haml, test.Format);
            var output = new StringWriter();
            output.NewLine = "\n";
            template.Render(output);

            var message = string.Format("{0} - {1}", test.GroupName, test.TestName);
            Assert.That(output.ToString(), Is.EqualTo(test.ExpectedHtml), message);
        }

        private Template CreateTemplate(string hamlTemplate, string htmlFormat)
        {
            var testFile = new FileInfo("test.haml");
            if (testFile.Exists) testFile.Delete();
            File.WriteAllText(testFile.FullName, hamlTemplate);
            var hamlOptions = new HamlOptions();
            if (htmlFormat == "html4")
                hamlOptions.HtmlVersion = HtmlVersion.Html4;
            else if (htmlFormat == "html5")
                hamlOptions.HtmlVersion = HtmlVersion.Html5;
            else if (htmlFormat == "xhtml")
                hamlOptions.HtmlVersion = HtmlVersion.XHtml;
            
            var templateEngine = new TemplateEngine(hamlOptions);
            var compiledTemplate = templateEngine.GetCompiledTemplate(new FileViewSource(testFile));
            return compiledTemplate.CreateTemplate();
        }
    }
}