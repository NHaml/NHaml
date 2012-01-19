using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Parser.Rules;
using NHaml4.Walkers.CodeDom;
using NHaml4.Tests.Mocks;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeHtmlAttributeCollectionWalker_Tests
    {
        [Test]
        [TestCase("()", "")]
        [TestCase("(a='b')", " a='b'")]
        [TestCase("(a='b' c='d')", " a='b' c='d'")]
        [TestCase("(class='class1')", "")]
        [TestCase("(id='id1')", "")]
        public void Walk_EmptyAttributeCollection_WritesCorrectAttributes(string hamlLine, string expectedTag)
        {
            var node = new HamlNodeHtmlAttributeCollection(0, hamlLine);

            var builder = new ClassBuilderMock();
            new HamlNodeHtmlAttributeCollectionWalker(builder, new HamlOptions())
                .Walk(node);

            Assert.That(builder.Build(""), Is.EqualTo(expectedTag));
        }

        [Test]
        [TestCase("(checked=true)", " checked='checked'", HtmlVersion.XHtml)]
        [TestCase("(checked)", " checked='checked'", HtmlVersion.XHtml)]
        [TestCase("(checked=false)", "", HtmlVersion.XHtml)]
        [TestCase("(checked=true)", " checked", HtmlVersion.Html5)]
        [TestCase("(checked)", " checked", HtmlVersion.Html5)]
        public void Walk_BooleanAttribute_WritesCorrectAttributes(string hamlLine, string expectedTag, HtmlVersion htmlVersion)
        {
            var node = new HamlNodeHtmlAttributeCollection(0, hamlLine);

            var builder = new ClassBuilderMock();
            var options = new HamlOptions { HtmlVersion = htmlVersion };
            new HamlNodeHtmlAttributeCollectionWalker(builder, options).Walk(node);

            Assert.That(builder.Build(""), Is.EqualTo(expectedTag));
        }
    }
}
