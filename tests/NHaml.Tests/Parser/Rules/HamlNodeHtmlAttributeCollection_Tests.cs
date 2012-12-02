using System.Linq;
using NUnit.Framework;
using NHaml.Parser.Rules;

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
    }
}
