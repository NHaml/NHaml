using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHaml4.TemplateBase;
using NUnit.Framework;

namespace NHaml4.Tests.TemplateBase
{
    [TestFixture]
    public class Template_Tests
    {
        private class DummyTemplate : Template
        {
            protected override void PreRender(TextWriter textWriter)
            {
                textWriter.Write("PreRender");
            }
            protected override void CoreRender(TextWriter textWriter)
            {
                textWriter.Write("CoreRender");
            }
            protected override void PostRender(TextWriter textWriter)
            {
                textWriter.Write("PostRender");
            }

            public void RenderAttributeIfValueNotNull(TextWriter writer, string attSchema, string attName, string attValue)
            {
                base.RenderAttributeIfValueNotNull(writer, attSchema, attName, attValue);
            }
        }

        #region Render
        [Test]
        public void Render_NormalUse_CallsPreRender()
        {
            var writer = new StringWriter();
            new DummyTemplate().Render(writer);
            Assert.That(writer.ToString(), Is.StringContaining("PreRender"));
        }

        [Test]
        public void Render_NormalUse_CallsCoreRender()
        {
            var writer = new StringWriter();
            new DummyTemplate().Render(writer);
            Assert.That(writer.ToString(), Is.StringContaining("CoreRender"));
        }

        [Test]
        public void Render_NormalUse_CallsPostRender()
        {
            var writer = new StringWriter();
            new DummyTemplate().Render(writer);
            Assert.That(writer.ToString(), Is.StringContaining("PostRender"));
        }
        #endregion
    
        #region RenderAttributeIfValueNotNull
        [Test]
        [TestCase("", "Name", null, @"", Description="Null attribute value")]
        [TestCase("", "", "Value", @"", Description = "Empty attribute name")]
        [TestCase("", "Name", "Value", @" Name=""Value""", Description = "Empty attribute name")]
        [TestCase("Schema", "Name", "Value", @" Schema:Name=""Value""", Description = "Empty attribute name")]
        public void RenderAttributeIfValueNotNull_MultipleScenarios_GeneratesCorrectOutput(string attSchema, string attName, string attValue, string expectedOutput)
        {
            var writer = new StringWriter();
            new DummyTemplate().RenderAttributeIfValueNotNull(writer, attSchema, attName, attValue);
            Assert.That(writer.ToString(), Is.EqualTo(expectedOutput));
        }

        #endregion

    }
}
