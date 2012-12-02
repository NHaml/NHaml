using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHaml.TemplateBase;
using NUnit.Framework;

namespace NHaml.Tests.TemplateBase
{
    [TestFixture]
    public class Template_Tests
    {
        private class DummyTemplate : Template
        {
            public string RenderValueOrKeyAsString(IDictionary<string, object> dictionary, string keyName)
            {
                base.SetViewData(dictionary);
                return base.RenderValueOrKeyAsString(keyName);
            }

            public new string RenderAttributeNameValuePair(string name, string value, char quoteToUse)
            {
                return base.RenderAttributeNameValuePair(name, value, quoteToUse);
            }

            public string AppendSelfClosingTagSuffix(HtmlVersion htmlVersion)
            {
                base.SetHtmlVersion(htmlVersion);
                return base.AppendSelfClosingTagSuffix();
            }
        }
        
        #region RenderValueOrKeyAsString

        [Test]
        [TestCase("FakeKeyName", "KeyName")]
        [TestCase("RealKeyName", "RealValue")]
        public void RenderValueOrKeyAsString_RealVsFakeKey_RendersKeyOrValueCorrectly(string keyName, string expecedValue)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("RealKeyName", "RealValue");

            var template = new DummyTemplate();
            string result  = template.RenderValueOrKeyAsString(dictionary, keyName);

            Assert.That(result, Is.StringContaining(expecedValue));
        }

        #endregion

        #region RenderAttributeNameValuePair

        [Test]
        [TestCase("a", "b", " a=\"b\"")]
        [TestCase("", "a", "")]
        [TestCase("a", "false", "")]
        public void RenderAttributeNameValuePair_VaryingNameValuePairs_GeneratesCorrectValue(string name, string value, string expectedOutput)
        {
            var template = new DummyTemplate();
            string result = template.RenderAttributeNameValuePair(name, value, '\"');
            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase("checked", "true", HtmlVersion.XHtml, " checked=\"checked\"")]
        [TestCase("checked", "TRUE", HtmlVersion.XHtml, " checked=\"checked\"")]
        [TestCase("checked", "", HtmlVersion.XHtml, "")]
        [TestCase("checked", "false", HtmlVersion.XHtml, "")]
        [TestCase("checked", "FALSE", HtmlVersion.XHtml, "")]
        [TestCase("checked", "true", HtmlVersion.Html5, " checked")]
        [TestCase("checked", "", HtmlVersion.Html5, "")]
        [TestCase("checked", "true", HtmlVersion.Html4, " checked")]
        [TestCase("checked", "", HtmlVersion.Html4, "")]
        public void RenderAttributeNameValuePair_BooleanAttribute_WritesCorrectAttributes(string name, string value, HtmlVersion htmlVersion, string expectedOutput)
        {
            var template = new DummyTemplate();
            template.SetHtmlVersion(htmlVersion);
            string result = template.RenderAttributeNameValuePair(name, value, '\"');

            Assert.That(result, Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase('\"')]
        [TestCase('\'')]
        public void RenderAttributeNameValuePair_VaryingQuoteTypes_RendersCorrectQuotes(char quoteToUse)
        {
            const string name = "name";
            const string value = "value";
            var template = new DummyTemplate();
            string result = template.RenderAttributeNameValuePair(name, value, quoteToUse);

            string expectedOutput = " " + name + "=" + quoteToUse + value + quoteToUse;
            Assert.That(result, Is.EqualTo(expectedOutput));
        }
        #endregion

        #region AppendSelfClosingTagSuffix

        [Test]
        [TestCase(HtmlVersion.Html4, ">")]
        [TestCase(HtmlVersion.XHtml, " />")]
        [TestCase(HtmlVersion.Html5, ">")]
        public void AppendSelfClosingTagSuffix_VaryingHtmlVersion_AppendsCorrectOutput(HtmlVersion htmlVersion, string expectedOutput)
        {
            var template = new DummyTemplate();
            string result = template.AppendSelfClosingTagSuffix(htmlVersion);

            Assert.That(result, Is.EqualTo(expectedOutput));
        }
        
        #endregion
    }
}
