using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.TemplateBase;
using NUnit.Framework;
using NHaml.Tests.Builders;
using NHaml4.Walkers.CodeDom;
using NHaml4;
using System.IO;

namespace HamlSpec
{
    public class SpecTestRunner
    {
        private int _totalNoTests;
        private int _totalNoTestsFailed;
        private IDictionary<string, IEnumerable<HamlSpec>> _hamlSpecs;

        public SpecTestRunner(string fileName)
        {
            _totalNoTests = 0;
            _totalNoTestsFailed = 0;

            _hamlSpecs = new HamlSpecLoader(fileName).GetTheHamlSpecTests();
        }

        public void OutputSummaryToConsole()
        {
            Console.WriteLine(string.Format("{0} OF {1} TESTS FAILED.", _totalNoTestsFailed, _totalNoTests));
        }

        public void ExecuteSpecs(string groupName)
        {
            var specs = _hamlSpecs[groupName];

            int errorCount = 0;
            foreach (var test in specs)
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
            int totalCount = specs.Count();
            Console.WriteLine(errorCount + " of " + totalCount + " scenarios failed.");

            _totalNoTests += totalCount;
            _totalNoTestsFailed += errorCount;

            Assert.AreEqual(0, errorCount, errorCount + " of " + totalCount + " scenarios failed.");
        }

        private void ExecuteSingleTest(HamlSpec test)
        {
            var template = CreateTemplate(test.Haml);
            var output = new StringWriter();
            output.NewLine = "\n";
            template.Render(output, GetHtmlVersion(test.Format), test.Locals);

            var message = string.Format("{0} - {1}\n\"{2}\"", test.GroupName, test.TestName, test.Haml);
            Assert.That(output.ToString(), Is.EqualTo(test.ExpectedHtml), message);
        }

        private HtmlVersion GetHtmlVersion(string htmlFormat)
        {
            if (htmlFormat == "html4")
                return HtmlVersion.Html4;
            else if (htmlFormat == "html5")
                return HtmlVersion.Html5;

            return HtmlVersion.XHtml;
        }

        private Template CreateTemplate(string hamlTemplate)
        {
            var viewSource = ViewSourceBuilder.Create(hamlTemplate);

            var hamlOptions = new HamlOptions();
            var templateEngine = new TemplateEngine(hamlOptions);
            var compiledTemplate = templateEngine.GetCompiledTemplate(viewSource);
            return compiledTemplate.CreateTemplate();
        }
    }
}
