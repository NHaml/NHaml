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
        [TestCase("#id.className", "class", "id")]
        [TestCase(".className#id", "class", "id")]
        //[TestCase(".className#id(att=value)", "class", "id", "att")]
        //[TestCase(".className#id{att => value}", "class", "id", "att")]
        public void Constructor_MultipleAttributes_OrderedCorrectly(string templateLine, params object[] args)
        {
            var tag = new HamlNodeTag(templateLine);

            for (int c = 0; c < args.Length; c++)
            {
                Assert.That((string) args[c], Is.EqualTo(tag.Attributes[c].Key));
            }
        }
    }
}
