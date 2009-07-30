using System;
using System.Diagnostics;
using System.IO;
using NHaml.Compilers.CSharp2;

using NUnit.Framework;

namespace NHaml.Tests
{
    public abstract class TestFixtureBase
    {
        protected string TemplatesFolder { get; set; }
        protected string ExpectedFolder { get; set; }

        protected TemplateEngine _templateEngine;

        protected TestFixtureBase()
        {
            TemplatesFolder = @"Functional\Templates\";
            ExpectedFolder = @"Functional\Expected\";

            Trace.Listeners.Clear();
        }

        [SetUp]
        public virtual void SetUp()
        {
            _templateEngine = new TemplateEngine {Options = {TemplateCompiler = new CSharp2TemplateCompiler()}};
            _templateEngine.TemplateContentProvider.PathSources.Add(TemplatesFolder);
        }

        protected void AssertRender( string templateName )
        {
            AssertRender(templateName,  new[]{templateName});
        }

        protected void AssertRender(params string[] templates)
        {
            AssertRender(templates[0], templates);
        }
        protected void AssertRender(string expectedName, string templateName)
        {
            AssertRender(expectedName, new[] { templateName });
        }

        protected void AssertRender(string expectedName, string[] templates)
        {
            var output = new StringWriter();
            var template = CreateTemplate(templates);

            template.Render( output );

            AssertRender( output, expectedName );
        }

        protected Template CreateTemplate(params string[] templates)
        {
            var stopwatch = Stopwatch.StartNew();

            var compiledTemplate = _templateEngine.Compile(templates);
            
            stopwatch.Stop();
            
            Debug.WriteLine(string.Format("Compile took {0} ms", stopwatch.ElapsedMilliseconds));
            
            return compiledTemplate.CreateInstance();
        }

        protected void AssertRender( StringWriter output, string expectedName )
        {
            Console.WriteLine( output );
            Assert.AreEqual( File.ReadAllText( ExpectedFolder + expectedName + ".xhtml" ), output.ToString() );
        }
    }
}