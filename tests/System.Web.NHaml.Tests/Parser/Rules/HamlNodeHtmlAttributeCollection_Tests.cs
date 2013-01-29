using System.Linq;
using System.Web.NHaml.Parser.Rules;
using NUnit.Framework;

namespace NHaml.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeHtmlAttributeCollection_Tests
    {
        [Test]
        [TestCase("(a='b')", 1)]
        [TestCase("(a='b' b=\"c\")", 2)]
        [TestCase("(a='b'\tb=\"c\")", 2)]
        public void Constructor_ValidAttributeStrings_AttributeCollectionContainsCorrectAttributeCount(
            string attributeString, int expectedAttributeCount)
        {
            var tag = new HamlNodeHtmlAttributeCollection(0, attributeString);

            Assert.That(tag.Children.Count(), Is.EqualTo(expectedAttributeCount));
        }

        [Test]
        [TestCase("(a='b')", "a='b'")]
        [TestCase("(a='b' b=\"c\")", "a='b'")]
        [TestCase("(a='b'\tb=\"c\")", "a='b'")]
        [TestCase("(a=\"b\"\tb=\"c\")", "a=\"b\"")]
        public void Constructor_ValidAttributeStrings_AttributeCollectionContainsCorrectAttributes(
            string attributeString, string expectedFirstAttribute)
        {
            var tag = new HamlNodeHtmlAttributeCollection(0, attributeString);

            Assert.That(tag.Children.First().Content, Is.EqualTo(expectedFirstAttribute));
        }

        [TestCase("(attrName=#{attrValue})", "attrName", "#{attrValue}")]
        public void Constructor_MixedBrackets_ParsesCorrectly(
            string attributeString, string expectedName, string expectedValue)
        {
            var tag = new HamlNodeHtmlAttributeCollection(0, attributeString);

            var firstChild = (HamlNodeHtmlAttribute)tag.Children.First();
            Assert.That(firstChild.Name, Is.EqualTo(expectedName));
            Assert.That(firstChild.Children.First().Content, Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("(a=b, c=d)", "a", "#{b}")]
        [TestCase("(a='b',c=d)", "a", "b")]
        [TestCase("(a=This.That, c=d)", "a", "#{This.That}")]
        public void Constructor_CommaSeparatedAttributes_ParsesCorrectly(
            string attributeString, string expectedName, string expectedValue)
        {
            var tag = new HamlNodeHtmlAttributeCollection(0, attributeString);

            var firstChild = (HamlNodeHtmlAttribute)tag.Children.First();
            Assert.That(firstChild.Name, Is.EqualTo(expectedName));
            Assert.That(firstChild.Children.First().Content, Is.EqualTo(expectedValue));
        }
    }
}
