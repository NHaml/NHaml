using System;
using System.Diagnostics;
using System.IO;
using NHaml.Compilers.CSharp2;

using NUnit.Framework;

namespace NHaml.Tests
{
    public abstract class TestFixtureBase
    {
        public const string TemplatesFolder = @"Templates\";
        public const string ExpectedFolder = @"Expected\";

        protected TemplateEngine _templateEngine;


        protected TestFixtureBase()
        {
            Trace.Listeners.Clear();
        }

        [SetUp]
        public virtual void SetUp()
        {
            _templateEngine = new TemplateEngine {Options = {TemplateCompiler = new CSharp2TemplateCompiler()}};
            _templateEngine.TemplateContentProvider.PathSources.Add("Templates");

        }

        protected void AssertRender( string templateName )
        {
            AssertRender( templateName, null );
        }

        protected void AssertRender( string templateName, string layoutName )
        {
            var expectedName = templateName;

            if( !string.IsNullOrEmpty( layoutName ) )
            {
                expectedName = layoutName;
            }

            AssertRender( templateName, layoutName, expectedName );
        }

        protected void AssertRender( string templateName, string layoutName, string expectedName )
        {
            var output = new StringWriter();
            var template = CreateTemplate( templateName, layoutName );

            template.Render( output );

            AssertRender( output, expectedName );
        }

        protected Template CreateTemplate( string templateName, string layoutName )
        {
            //var templatePath = string.Format("{0}{1}\\{2}.haml", TemplatesFolder, _primaryTemplatesFolder, templateName);

            //if( !File.Exists( templatePath ) )
            //{
            //    templatePath = string.Format("{0}{1}\\{2}.haml", TemplatesFolder, _secondaryTemplatesFolder, templateName);
            //}

            //if( !File.Exists( templatePath ) )
            //{
            //    templatePath = string.Format("{0}{1}.haml", TemplatesFolder, templateName);
            //}

            //if( !string.IsNullOrEmpty( layoutName ) )
            //{
            //    layoutName = string.Format("{0}{1}.haml", TemplatesFolder, layoutName);
            //}

            var stopwatch = Stopwatch.StartNew();

            var compiledTemplate = _templateEngine.Compile(templateName, layoutName);
            stopwatch.Stop();
            Debug.WriteLine(string.Format("Compile took {0} ms", stopwatch.ElapsedMilliseconds));
            return compiledTemplate.CreateInstance();
        }

        protected static void AssertRender( StringWriter output, string expectedName )
        {
            Console.WriteLine( output );
            Assert.AreEqual( File.ReadAllText( ExpectedFolder + expectedName + ".xhtml" ), output.ToString() );
        }
    }
}