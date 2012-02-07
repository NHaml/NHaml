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
            protected override void CoreRender(TextWriter textWriter)
            {
                textWriter.Write("CoreRender");
            }
        }

        #region Render
        [Test]
        public void Render_NormalUse_CallsCoreRender()
        {
            var writer = new StringWriter();
            new DummyTemplate().Render(writer);
            Assert.That(writer.ToString(), Is.StringContaining("CoreRender"));
        }
        #endregion

        #region RenderValueOrKeyAsString
        #endregion
    }
}
