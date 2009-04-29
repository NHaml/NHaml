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

        protected string _primaryTemplatesFolder;
        protected string _secondaryTemplatesFolder;

        protected TestFixtureBase()
        {
            Trace.Listeners.Clear();
        }

        [SetUp]
        public virtual void SetUp()
        {
            _templateEngine = new TemplateEngine { TemplateCompiler = new CSharp2TemplateCompiler() };

            _primaryTemplatesFolder = "CSharp2";
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
            var templatePath = string.Format("{0}{1}\\{2}.haml", TemplatesFolder, _primaryTemplatesFolder, templateName);

            if( !File.Exists( templatePath ) )
            {
                templatePath = string.Format("{0}{1}\\{2}.haml", TemplatesFolder, _secondaryTemplatesFolder, templateName);
            }

            if( !File.Exists( templatePath ) )
            {
                templatePath = string.Format("{0}{1}.haml", TemplatesFolder, templateName);
            }

            if( !string.IsNullOrEmpty( layoutName ) )
            {
                layoutName = string.Format("{0}{1}.haml", TemplatesFolder, layoutName);
            }

            var compiledTemplate = _templateEngine.Compile( templatePath, layoutName );
            return compiledTemplate.CreateInstance();
        }

        protected static void AssertRender( StringWriter output, string expectedName )
        {
            //Clipboard.SetText(output.ToString());
            Console.WriteLine( output );

            Assert.AreEqual( File.ReadAllText( ExpectedFolder + expectedName + ".xhtml" ), output.ToString() );
        }
    }
}