using System.Linq;
using NHaml.Parser;
using NUnit.Framework;
using NHaml.IO;
using NHaml.Parser.Rules;
using NHaml.Parser.Exceptions;

namespace NHaml.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeTag_Tests
    {
        [Test]
        [TestCase("p", "p")]
        [TestCase("", "div")]
        [TestCase("p#id_goes_here", "p")]
        [TestCase("p.class_goes_here", "p")]
        [TestCase("h1", "h1")]
        [TestCase("br/", "br")]
        public void Constructor_SimpleTags_GeneratesCorrectTagName(string templateLine, string expectedTagName)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));
            Assert.That(tag.TagName, Is.EqualTo(expectedTagName));
        }

        [Test]
        [TestCase("p", "", 0)]
        [TestCase("p#id", "id", 1)]
        [TestCase("p#id1#id2", "id1", 2)]
        [TestCase("p#id.className", "id", 1)]
        public void Constructor_SimpleTags_GeneratesCorrectIdChildNodes(string templateLine, string expectedFirstTagId, int expectedCount)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            var idTags = tag.Children.OfType<HamlNodeTagId>();

            Assert.That(idTags.Count(), Is.EqualTo(expectedCount));
            string att = idTags.FirstOrDefault() != null ? idTags.First().Content : "";
            Assert.That(att, Is.EqualTo(expectedFirstTagId));
        }

        [Test]
        [TestCase("p", "", 0)]
        [TestCase("p.test", "test", 1)]
        [TestCase("p#id.test", "test", 1)]
        [TestCase("p.test#id", "test", 1)]
        [TestCase("p.test#id.test2", "test", 2)]
        public void Constructor_SimpleTags_GeneratesCorrectClassChildNodes(string templateLine, string expectedFirstClass, int expectedCount)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            var classChildren = tag.Children.OfType<HamlNodeTagClass>();

            Assert.That(classChildren.Count(), Is.EqualTo(expectedCount));
            string att = classChildren.FirstOrDefault() != null ? classChildren.First().Content : "";
            Assert.That(att, Is.EqualTo(expectedFirstClass));
        }

        [Test]
        [TestCase("p", false)]
        [TestCase("br/", true)]
        [TestCase("zzz(a='b')/", true)]
        public void Constructor_SimpleTags_DeterminesSelfClosingCorrectly(string templateLine, bool expectedSelfClosing)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));
            Assert.That(tag.IsSelfClosing, Is.EqualTo(expectedSelfClosing));
        }

        [Test]
        [TestCase("tag", "", "tag")]
        [TestCase("ns:tag", "ns", "tag")]
        public void Constructor_TagWithNamespace_DeterminesTagAndNamespaceCorrectly(string templateLine,
            string expectedNamespace, string expectedTag)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));
            Assert.That(tag.Namespace, Is.EqualTo(expectedNamespace));
            Assert.That(tag.TagName, Is.EqualTo(expectedTag));

        }

        [Test]
        public void Constructor_InlineContent_GeneratesCorrectChildTag()
        {
            const string templateLine = "p Hello world";
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            Assert.That(tag.Children.First(), Is.InstanceOf<HamlNodeTextContainer>());
            const string expectedText = "Hello world";
            Assert.That(((HamlNodeTextContainer)tag.Children.First()).Content, Is.EqualTo(expectedText));
        }

        [Test]
        public void Constructor_HtmlStyleAttribute_GeneratesHtmlAttributeCollectionTag()
        {
            const string templateLine = "p(a='b')";
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            Assert.That(tag.Children.First(), Is.InstanceOf<HamlNodeHtmlAttributeCollection>());
        }

        [Test]
        public void Constructor_LegacyMangledRubyStyleAttribute_GeneratesHtmlAttributeCollectionTag()
        {
            const string templateLine = "p{a='b'}";
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            Assert.That(tag.Children.First(), Is.InstanceOf<HamlNodeHtmlAttributeCollection>());
        }

        [Test]
        [TestCase("p(a='b')", "(a='b')", 1)]
        [TestCase("p(a='b)')", "(a='b)')", 1)]
        [TestCase("p(a=\"b\")", "(a=\"b\")", 1)]
        [TestCase("p(a='b\"')", "(a='b\"')", 1)]
        [TestCase("p(a='b')Content", "(a='b')", 2)]
        public void Constructor_HtmlStyleAttribute_AttributeCollectionContainsCorrectContent(
            string templateLine, string expectedAttributeContent, int expectedaAttrCount)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            Assert.That(tag.Children.First().Content, Is.EqualTo(expectedAttributeContent));
            Assert.That(tag.Children.Count(), Is.EqualTo(expectedaAttrCount));
        }

        [Test]
        public void Constructor_HtmlStyleAttributeWithContent_GeneratesCorrectChildren()
        {
            const string templateLine = "p(a='b')Content";
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            const string expectedAttrContent = "(a='b')";
            Assert.That(tag.Children.First().Content, Is.EqualTo(expectedAttrContent));
            const string expectedStringContent = "Content";
            Assert.That(tag.Children.ToList()[1].Content, Is.EqualTo(expectedStringContent));
        }

        [Test]
        public void Constructor_MalformedHtmlStyleAttributes_ThrowsMalformedTagException()
        {
            const string templateLine = "p(a='b'";
            var line = new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0);

            Assert.Throws<HamlMalformedTagException>(() => new HamlNodeTag(line));
        }

        [Test]
        [TestCase("p>", WhitespaceRemoval.Surrounding)]
        [TestCase("p<", WhitespaceRemoval.Internal)]
        [TestCase("p", WhitespaceRemoval.None)]
        public void Walk_TagWithWhitespaceSupression_SetsCorrectFlag(string templateLine, WhitespaceRemoval expectedSetting)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            Assert.That(tag.WhitespaceRemoval, Is.EqualTo(expectedSetting));
        }

        [Test]
        [TestCase("p #variable", "#variable")]
        public void Walk_TagWithWhitespaceSupression_SetsCorrectFlag(string templateLine, string expectedVariableContent)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

            Assert.That(tag.Children.First().Children.First().Content, Is.EqualTo(expectedVariableContent));
        }
    }
}
