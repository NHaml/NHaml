using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

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

            var resources = new TemplateCompileResources(_templateEngine.Options.TemplateBaseType, templatePath);

            var compiledTemplate1 = _templateEngine.GetCompiledTemplate( resources );
            var compiledTemplate2 = _templateEngine.GetCompiledTemplate( resources );

            Assert.AreSame( compiledTemplate1, compiledTemplate2 );
        }

        [Test]
        public void TemplatesWithLayoutsAreCached()
        {
            var templatePath = TemplatesFolder + @"CSharp2\Welcome.haml";
            var layoutTemplatePath = TemplatesFolder + @"Application.haml";

            var resources = new TemplateCompileResources(_templateEngine.Options.TemplateBaseType, 
                new List<string> { templatePath, layoutTemplatePath });

            var compiledTemplate1 = _templateEngine.GetCompiledTemplate(resources);
            var compiledTemplate2 = _templateEngine.GetCompiledTemplate(resources);

            Assert.AreSame( compiledTemplate1, compiledTemplate2 );
        }

     

        [Test]
        public void TemplatesWithDifferentLayoutsAreCachedSeperate()
        {
            var templatePath = TemplatesFolder + @"CSharp2\Welcome.haml";
            var layoutTemplatePath1 = TemplatesFolder + @"Application.haml";
            var layoutTemplatePath2 = TemplatesFolder + @"ApplicationSimple.haml";

            var resources1 = new TemplateCompileResources(_templateEngine.Options.TemplateBaseType,
                new List<string> { templatePath, layoutTemplatePath1 });
            var resources2 = new TemplateCompileResources(_templateEngine.Options.TemplateBaseType, 
                new List<string> { templatePath, layoutTemplatePath2 });

            var compiledTemplate1 = _templateEngine.GetCompiledTemplate(resources1);
            var compiledTemplate2 = _templateEngine.GetCompiledTemplate(resources2);

            Assert.AreNotSame( compiledTemplate1, compiledTemplate2 );
        }

        public abstract class MockDataView<TViewData> : Template
        {
            public TViewData ViewData { get; set; }

        }

    }
}