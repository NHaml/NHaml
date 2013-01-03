using System;
using System.Dynamic;
using System.Web.NHaml;
using System.Web.NHaml.Compilers;
using System.Web.NHaml.IO;
using System.Web.NHaml.Parser;
using System.Web.NHaml.TemplateBase;
using System.Web.NHaml.TemplateResolution;
using System.Web.NHaml.Walkers.CodeDom;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;
using NHaml.Tests.Builders;
using System.IO;
using System.Collections.Generic;

namespace NHaml.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class CustomBaseClass_IntegrationTests
    {
        private TemplateEngine _templateEngine;

        [SetUp]
        public void SetUp()
        {
            var templateFactoryFactory = new TemplateFactoryFactory(
                new FileTemplateContentProvider(),
                new HamlTreeParser(new HamlFileLexer()),
                new HamlDocumentWalker(new CodeDomClassBuilder()),
                new CodeDomTemplateCompiler(new CSharp2TemplateTypeBuilder()),
                new List<string>(),
                new List<string>());            
            _templateEngine = new TemplateEngine(new SimpleTemplateCache(), templateFactoryFactory);
        }

        [Test]
        public void SimpleIntegrationTest()
        {
            // Arrange
            const string templateContent = @"This is a test";
            var viewSource = ViewSourceBuilder.Create(templateContent);
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource, typeof(CustomTemplateBase));

            // Act
            Template template = templateFactory.CreateTemplate();
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual("This is a test", textWriter.ToString());
        }

        [Test]
        public void TemplateWithDynamicModel_RenderNullString_ThrowsArgumentException()
        {
            // Arrange
            const string templateContent = @"=Model.NullValue";
            var viewSource = ViewSourceBuilder.Create(templateContent);
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource, typeof(CustomTemplateBase));

            // Act
            var template = (CustomTemplateBase)templateFactory.CreateTemplate();
            template.Model.NullValue = null;
            var textWriter = new StringWriter();
            Assert.Throws<RuntimeBinderException>(() => template.Render(textWriter));
        }

        [Test]
        public void TemplateWithDynamicModel_RenderNullWithCastString_RendersEmptyString()
        {
            // Arrange
            const string templateContent = @"=(string)Model.NullValue";
            var viewSource = ViewSourceBuilder.Create(templateContent);
            var templateFactory = _templateEngine.GetCompiledTemplate(viewSource, typeof(CustomTemplateBase));

            // Act
            var template = (CustomTemplateBase)templateFactory.CreateTemplate();
            template.Model.NullValue = null;
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual(string.Empty, textWriter.ToString());
        }
    }

    public class CustomTemplateBase : Template
    {
        public dynamic Model { get; set; }

        public CustomTemplateBase()
        {
            Model = new ExpandoObject();
        }
    }
}
