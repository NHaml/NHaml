using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.TemplateBase;
using NHaml4.Walkers.CodeDom;

namespace NHaml4.Tests.TemplateBase
{
    [TestFixture]
    public class HamlDocTypeFactory_Tests
    {
        [Test]
        [TestCase("", HtmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">")]
        [TestCase("strict", HtmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">")]
        [TestCase("frameset", HtmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Frameset//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd"">")]
        [TestCase("5", HtmlVersion.XHtml, @"<!DOCTYPE html>")]
        [TestCase("1.1", HtmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.1//EN"" ""http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd"">")]
        [TestCase("basic", HtmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML Basic 1.1//EN"" ""http://www.w3.org/TR/xhtml-basic/xhtml-basic11.dtd"">")]
        [TestCase("mobile", HtmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//WAPFORUM//DTD XHTML Mobile 1.2//EN"" ""http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd"">")]
        [TestCase("RDFa", HtmlVersion.XHtml, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML+RDFa 1.0//EN"" ""http://www.w3.org/MarkUp/DTD/xhtml-rdfa-1.dtd"">")]
        [TestCase("", HtmlVersion.Html4, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">")]
        [TestCase("strict", HtmlVersion.Html4, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01//EN"" ""http://www.w3.org/TR/html4/strict.dtd"">")]
        [TestCase("frameset", HtmlVersion.Html4, @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Frameset//EN"" ""http://www.w3.org/TR/html4/frameset.dtd"">")]
        [TestCase("", HtmlVersion.Html5, @"<!DOCTYPE html>")]
        [TestCase("XML", HtmlVersion.XHtml, "<?xml version='1.0' encoding='utf-8' ?>")]
        [TestCase("XML blah", HtmlVersion.XHtml, "<?xml version='1.0' encoding='blah' ?>")]
        public void Walk_ReturnsCorrectDocType(string docTypeId, HtmlVersion htmlVersion, string expectedDocType)
        {
            var docType = DocTypeFactory.GetDocType(docTypeId, htmlVersion);
            Assert.That(docType, Is.EqualTo(expectedDocType));
        }
    }
}
