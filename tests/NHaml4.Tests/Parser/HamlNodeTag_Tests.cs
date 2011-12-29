using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser;
using NUnit.Framework;

namespace NHaml4.Tests.Parser
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
            var tag = new HamlNodeTag(templateLine);
            Assert.That(tag.TagName, Is.EqualTo(expectedTagName));
        }

        [Test]
        [TestCase("p", "")]
        [TestCase("p#id", "id")]
        [TestCase("p#id1#id2", "id2")]
        [TestCase("p#id.className", "id")]
        public void Constructor_SimpleTags_GeneratesCorrectId(string templateLine, string expectedTagId)
        {
            var tag = new HamlNodeTag(templateLine);

            string att = tag.Attributes.Any(x => x.Key == "id")
                ? tag.Attributes.First(x => x.Key == "id").Value
                : "";
            Assert.That(att, Is.EqualTo(expectedTagId));
        }

        [Test]
        [TestCase("p", "")]
        [TestCase("p.test", "test")]
        [TestCase("p#id.test", "test")]
        [TestCase("p.test#id", "test")]
        public void Constructor_SimpleTags_GeneratesCorrectClassName(string templateLine, string expectedClassNames)
        {
            var tag = new HamlNodeTag(templateLine);
            
            string att = tag.Attributes.Any(x => x.Key == "class")
                ? tag.Attributes.First(x => x.Key == "class").Value
                : "";
            Assert.That(att, Is.EqualTo(expectedClassNames));
        }

        [Test]
        [TestCase("p", false)]
        [TestCase("br/", true)]
        public void Constructor_SimpleTags_DeterminesSelfClosingCorrectly(string templateLine, bool expectedSelfClosing)
        {
            var tag = new HamlNodeTag(templateLine);
            Assert.That(tag.IsSelfClosing, Is.EqualTo(expectedSelfClosing));
        }

        [Test]
        [TestCase("tag", "", "tag")]
        [TestCase("ns:tag", "ns", "tag")]
        public void Constructor_TagWithNamespace_DeterminesTagAndNamespaceCorrectly(string templateLine,
            string expectedNamespace, string expectedTag)
        {
            var tag = new HamlNodeTag(templateLine);
            Assert.That(tag.Namespace, Is.EqualTo(expectedNamespace));
            Assert.That(tag.TagName, Is.EqualTo(expectedTag));

        }

        [Test]
        [TestCase("#id.className", "class", "id", "")]
        [TestCase(".className#id", "class", "id", "")]
        //[TestCase(".className#id(att=value)", "class", "id", "att")]
        //[TestCase(".className#id{att => value}", "class", "id", "att")]
        public void Constructor_MultipleAttributes_OrderedCorrectly(string templateLine, string att0, string att1, string att2)
        {
            var tag = new HamlNodeTag(templateLine);

            Assert.That(tag.Attributes[0].Key, Is.EqualTo(att0));
            Assert.That(tag.Attributes[1].Key, Is.EqualTo(att1));
            if (!string.IsNullOrEmpty(att2))
                Assert.That(tag.Attributes[2].Key, Is.EqualTo(att2));
        }

        [Test]
        public void Constructor_InlineContent_GeneratesCorrectChildTag()
        {
            const string templateLine = "p Hello world";
            var tag = new HamlNodeTag(templateLine);

            Assert.That(tag.Children[0], Is.InstanceOf<HamlNodeText>());
            const string expectedText = "Hello world";
            Assert.That(((HamlNodeText)tag.Children[0]).Text, Is.EqualTo(expectedText));
        }
    }
}
