﻿using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Tests.Builders;
using NUnit.Framework;
using NHaml4.TemplateBase;
using System.IO;
using NHaml4.Configuration;

namespace NHaml4.IntegrationTests
{
    [TestFixture]
    public class XmlConfigurator_IntegrationTests
    {
        [Test]
        public void XmlConfigurator_DefaultUsage_CompilesValidTemplate()
        {
            // Arrange
            string templateContent = @"This is a test";

            var viewSource = ViewSourceBuilder.Create(templateContent);

            // Act
            var templateEngine = XmlConfigurator.GetTemplateEngine();
            var compiledTemplateFactory = templateEngine.GetCompiledTemplate(viewSource);
            Template template = compiledTemplateFactory.CreateTemplate();
            var textWriter = new StringWriter();
            template.Render(textWriter);

            // Assert
            Assert.AreEqual("This is a test", textWriter.ToString());
        }

        [Test]
        public void GetTemplateEngine_BasicHamlConfig_ContainsExtraUsingStatement()
        {
            var viewSource = ViewSourceBuilder.Create("= NHaml4.IntegrationTests.XmlConfigurator_IntegrationTests.TestRenderMethod()");
            var templateEngine = XmlConfigurator.GetTemplateEngine();
            var templateFactory = templateEngine.GetCompiledTemplate(viewSource);
            var template = templateFactory.CreateTemplate();
            var textWriter = new StringWriter();
            template.Render(textWriter);
            Assert.That(textWriter.ToString(), Is.StringMatching("Test"));
        }

        public static string TestRenderMethod()
        {
            return "Test";
        }
    }
}