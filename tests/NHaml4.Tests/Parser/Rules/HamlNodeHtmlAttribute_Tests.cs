using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Parser.Rules;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeHtmlAttribute_Tests
    {
        [Test]
        [TestCase("a='b'", "a", "b")]
        [TestCase("a", "a", "")]
        public void Constructor_NormalUse_GeneratesCorrectNameValuePair(string input, string name, string value)
        {
            var node = new HamlNodeHtmlAttribute(0, input);
            Assert.That(node.Name, Is.EqualTo(name));
            if (string.IsNullOrEmpty(value))
                Assert.That(node.Children, Is.Empty);
            else
                Assert.That(node.Children[0].Children[0].Content, Is.EqualTo(value));
        }


        [Test]
        public void Constructor_NormalUse_RepresentsValueAsTextContainer()
        {
            var node = new HamlNodeHtmlAttribute(0, "a='b'");
            Assert.That(node.Children[0], Is.InstanceOf<HamlNodeTextContainer>());
        }

        [Test]
        public void Constructor_NormalUse_RemovesQuoteMarks()
        {
            var node = new HamlNodeHtmlAttribute(0, "a='b'");
            Assert.That(node.Children[0].Children[0], Is.InstanceOf<HamlNodeTextLiteral>());
            Assert.That(node.Children[0].Children[0].Content, Is.EqualTo("b"));
        }

        [Test]
        public void Constructor_ValueWithoutQuotes_ConvertsValueToVariable()
        {
            var node = new HamlNodeHtmlAttribute(0, "a=b");
            Assert.That(node.Children[0].Children[0], Is.InstanceOf<HamlNodeTextVariable>());
            Assert.That(node.Children[0].Children[0].Content, Is.EqualTo("#{b}"));
        }

        public void Constructor_MalformedAttribute_ThrowsException()
        {
            Assert.Throws<HamlMalformedTagException>(() => new HamlNodeHtmlAttribute(0, "=b"));

        }

        [Test]
        [TestCase("a='b'", '\'')]
        [TestCase("a=\"b\"", '\"')]
        [TestCase("a=b", '\'')]
        public void Constructor_ValidAttributeStrings_ExtractsQuoteCharCorrectly(
            string attributeString, char expectedQuoteChar)
        {
            var tag = new HamlNodeHtmlAttribute(0, attributeString);

            Assert.That(tag.QuoteChar, Is.EqualTo(expectedQuoteChar));
        }
    }
}
