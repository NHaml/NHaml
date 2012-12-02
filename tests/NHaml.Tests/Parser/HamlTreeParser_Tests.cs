using System;
using NHaml.IO;
using NUnit.Framework;
using NHaml.Parser;
using NHaml.Tests.Builders;
using NHaml.Parser.Rules;
using NHaml.Parser.Exceptions;
using System.Linq;

namespace NHaml.Tests.Parser
{
    [TestFixture]
    public class HamlTreeParser_Tests
    {
        private HamlTreeParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new HamlTreeParser(new HamlFileLexer());
        }

        [Test]
        public void ParseViewSource_SingleLineTemplate_ReturnsHamlTree()
        {
            var viewSource = ViewSourceBuilder.Create("Test");
            var result = _parser.ParseViewSource(viewSource);
            Assert.IsInstanceOf(typeof(HamlDocument), result);
        }

        [Test]
        [TestCase("Test content", typeof(HamlNodeTextContainer))]
        [TestCase("%p", typeof(HamlNodeTag))]
        [TestCase("-#comment", typeof(HamlNodeHamlComment))]
        [TestCase("/comment", typeof(HamlNodeHtmlComment))]
        [TestCase("= Test", typeof(HamlNodeEval))]
        public void ParseDocumentSource_DifferentLineTypes_CreatesCorrectTreeNodeTypes(string template, Type nodeType)
        {
            var result = _parser.ParseDocumentSource(template, "");
            Assert.IsInstanceOf(nodeType, result.Children.First());
        }

        [Test]
        [TestCase("", 0)]
        [TestCase("Test", 1)]
        [TestCase("Test\nTest", 3)]
        [TestCase("Test\n  Test", 1)]
        public void ParseDocumentSource_SingleLevelTemplates_TreeContainsCorrectNoOfChildren(string template, int expectedChildrenCount)
        {
            var result = _parser.ParseDocumentSource(template, "");
            Assert.That(result.Children.Count(), Is.EqualTo(expectedChildrenCount));
        }

        [Test]
        public void ParseDocumentSource_MultiLineTemplates_AddsLineBreakNode()
        {
            const string template = "Line1\nLine2";
            var result = _parser.ParseDocumentSource(template, "");
            Assert.That(result.Children.ToList()[1].Content, Is.EqualTo("\n"));
        }

        [Test]
        [TestCase("Test\n  Test", 1)]
        [TestCase("Test\n  Test\n  Test", 1)]
        [TestCase("Test\n  Test\n    Test", 1)]
        [TestCase("Test\n  Test\nTest", 3)]
        public void ParseDocumentSource_MultiLevelTemplates_TreeContainsCorrectNoChildren(string template, int expectedChildren)
        {
            var result = _parser.ParseDocumentSource(template, "");
            Assert.AreEqual(expectedChildren, result.Children.Count());
        }

        public void ParseDocumentSource_FileNameSpecified_DocumentContainsMatchingFileName()
        {
            const string fileName = "FileName";
            var result = _parser.ParseDocumentSource("", fileName);
            Assert.That(result.Content, Is.EqualTo(fileName));
        }

        [Test]
        public void ParseDocumentSource_NestedContent_PlacesLineBreaksCorrectly()
        {
            string template = "%p Line 1\n%p\n  Line 2\n%p Line 3";
            var result = _parser.ParseDocumentSource(template, "");

            var children = result.Children.ToList();

            Assert.That(children[1].Content, Is.EqualTo("\n"));
            Assert.That(children[2].Children.First().Content, Is.EqualTo("\n"));
            Assert.That(children[2].Children.Count(), Is.EqualTo(2));
            Assert.That(children[3].Content, Is.EqualTo("\n"));
        }

        [Test]
        public void ParseDocumentSource_InlineContent_PlacesLineBreaksCorrectly()
        {
            string template = "%p =DateTime.Now()";
            var result = _parser.ParseDocumentSource(template, "");

            var children = result.Children.ToList();
            Assert.That(children[0].Children.Count(), Is.EqualTo(1));
            Assert.That(children[0].Children.First().Content, Is.EqualTo("DateTime.Now()"));
        }

        [Test]
        public void ParseHamlFile_UnknownRuleType_ThrowsUnknownRuleException()
        {
            var line = new HamlLine("", HamlRuleEnum.Unknown, "", 0);

            var file = new HamlFile("");
            file.AddLine(line);
            Assert.Throws<HamlUnknownRuleException>(() => _parser.ParseHamlFile(file));           
        }
    }
}
