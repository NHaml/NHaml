using System.Linq;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class TemplateEngineTests : TestFixtureBase
    {
        [Test]
        public void DuplicateUsings()
        {
            _templateEngine.Options.AddUsing( "System" );

            Assert.AreEqual( 7, _templateEngine.Options.Usings.Count() );
        }

        [Test]
        public void TemplatesAreCached()
        {
            var templatePath = TemplatesFolder + @"CSharp2\AttributeEval.haml";

            var compiledTemplate1 = _templateEngine.Compile( templatePath );
            var compiledTemplate2 = _templateEngine.Compile( templatePath );

            Assert.AreSame( compiledTemplate1, compiledTemplate2 );
        }

        [Test]
        public void TemplatesWithLayoutsAreCached()
        {
            var templatePath = TemplatesFolder + @"CSharp2\Welcome.haml";
            var layoutTemplatePath = TemplatesFolder + @"Application.haml";

            var compiledTemplate1 = _templateEngine.Compile( templatePath, layoutTemplatePath );
            var compiledTemplate2 = _templateEngine.Compile( templatePath, layoutTemplatePath );

            Assert.AreSame( compiledTemplate1, compiledTemplate2 );
        }

     

        [Test]
        public void TemplatesWithDifferentLayoutsAreCachedSeperate()
        {
            var templatePath = TemplatesFolder + @"CSharp2\Welcome.haml";
            var layoutTemplatePath1 = TemplatesFolder + @"Application.haml";
            var layoutTemplatePath2 = TemplatesFolder + @"ApplicationSimple.haml";

            var compiledTemplate1 = _templateEngine.Compile( templatePath, layoutTemplatePath1 );
            var compiledTemplate2 = _templateEngine.Compile( templatePath, layoutTemplatePath2 );

            Assert.AreNotSame( compiledTemplate1, compiledTemplate2 );
        }

        public abstract class MockDataView<TViewData> : Template
        {
            public TViewData ViewData { get; set; }

        }

    }
}