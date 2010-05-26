using System;
using System.Diagnostics;
using System.IO;
using NHaml.Core.Compilers;
using NUnit.Framework;
using NHaml.Core.Template;

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
            _templateEngine = new TemplateEngine {Options = { TemplateCompilerType = typeof(CSharpClassBuilder) }};
            _templateEngine.Options.TemplateContentProvider.AddPathSource(TemplatesFolder);
        }

        protected void AssertRender( string templateName )
        {
            AssertRender(templateName, null);
        }

        protected void AssertRender(string templateName, string masterName)
        {
            AssertRender(templateName, masterName, templateName);
        }

        protected void AssertRender(string templateName, string masterName, string expectedOutputName)
        {
            using (var output = new StringWriter())
            {
                var templ = CreateTemplate(templateName, masterName);

                templ.Render( output );

                AssertRender( output, expectedOutputName );
            }
        }

        protected Template CreateTemplate(string templateName)
        {
            return CreateTemplate(templateName, null);
        }

        protected Template CreateTemplate( string templateName, string masterFile)
        {
            var stopwatch = Stopwatch.StartNew();

            var compiledTemplate = _templateEngine.Compile(templateName,masterFile);
            
            stopwatch.Stop();
            
            Debug.WriteLine(string.Format("Compile took {0} ms", stopwatch.ElapsedMilliseconds));
            
            return compiledTemplate.CreateInstance();
        }

        protected void AssertRender( StringWriter output, string expectedName )
        {
            //Console.WriteLine( output );
            Assert.AreEqual( File.ReadAllText( ExpectedFolder + expectedName + ".xhtml" ).TrimEnd(), output.ToString().TrimEnd() );
        }
    }
}