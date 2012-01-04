using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser;
using NUnit.Framework;
using NHaml4.IO;
using NHaml4.Parser.Rules;

namespace NHaml4.Tests.Parser.Rules
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
            var tag = new HamlNodeTag(new HamlLine(templateLine));
            Assert.That(tag.TagName, Is.EqualTo(expectedTagName));
        }

        [Test]
        [TestCase("p", "", 0)]
        [TestCase("p#id", "id", 1)]
        [TestCase("p#id1#id2", "id1", 2)]
        [TestCase("p#id.className", "id", 1)]
        public void Constructor_SimpleTags_GeneratesCorrectIdChildNodes(string templateLine, string expectedFirstTagId, int expectedCount)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine));

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
            var tag = new HamlNodeTag(new HamlLine(templateLine));

            var classChildren = tag.Children.OfType<HamlNodeTagClass>();

            Assert.That(classChildren.Count(), Is.EqualTo(expectedCount));
            string att = classChildren.FirstOrDefault() != null ? classChildren.First().Content : "";
            Assert.That(att, Is.EqualTo(expectedFirstClass));
        }

        [Test]
        [TestCase("p", false)]
        [TestCase("br/", true)]
        public void Constructor_SimpleTags_DeterminesSelfClosingCorrectly(string templateLine, bool expectedSelfClosing)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine));
            Assert.That(tag.IsSelfClosing, Is.EqualTo(expectedSelfClosing));
        }

        [Test]
        [TestCase("tag", "", "tag")]
        [TestCase("ns:tag", "ns", "tag")]
        public void Constructor_TagWithNamespace_DeterminesTagAndNamespaceCorrectly(string templateLine,
            string expectedNamespace, string expectedTag)
        {
            var tag = new HamlNodeTag(new HamlLine(templateLine));
            Assert.That(tag.Namespace, Is.EqualTo(expectedNamespace));
            Assert.That(tag.TagName, Is.EqualTo(expectedTag));

        }

        [Test]
        public void Constructor_InlineContent_GeneratesCorrectChildTag()
        {
            const string templateLine = "p Hello world";
            var tag = new HamlNodeTag(new HamlLine(templateLine));

            Assert.That(tag.Children[0], Is.InstanceOf<HamlNodeText>());
            const string expectedText = "Hello world";
            Assert.That(((HamlNodeText)tag.Children[0]).Content, Is.EqualTo(expectedText));
        }
    }
}
