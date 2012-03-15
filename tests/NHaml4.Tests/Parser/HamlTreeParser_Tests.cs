using System;
using NHaml4.IO;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml.Tests.Builders;
using NHaml4.Parser.Rules;
using NHaml4.Parser.Exceptions;
using System.Linq;

namespace NHaml4.Tests.Parser
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
            var result = _parser.ParseDocumentSource(template);
            Assert.IsInstanceOf(nodeType, result.Children.First());
        }

        [Test]
        [TestCase("", 0)]
        [TestCase("Test", 1)]
        [TestCase("Test\nTest", 3)]
        [TestCase("Test\n  Test", 1)]
        public void ParseDocumentSource_SingleLevelTemplates_TreeContainsCorrectNoOfChildren(string template, int expectedChildrenCount)
        {
            var result = _parser.ParseDocumentSource(template);
            Assert.That(result.Children.Count(), Is.EqualTo(expectedChildrenCount));
        }

        [Test]
        public void ParseDocumentSource_MultiLineTemplates_AddsLineBreakNode()
        {
            string template = "Line1\nLine2";
            var result = _parser.ParseDocumentSource(template);
            Assert.That(result.Children.ToList()[1].Content, Is.EqualTo("\n"));
        }

        [Test]
        [TestCase("Test\n  Test", 1)]
        [TestCase("Test\n  Test\n  Test", 1)]
        [TestCase("Test\n  Test\n    Test", 1)]
        [TestCase("Test\n  Test\nTest", 3)]
        public void ParseDocumentSource_MultiLevelTemplates_TreeContainsCorrectNoChildren(string template, int expectedChildren)
        {
            var result = _parser.ParseDocumentSource(template);
            Assert.AreEqual(expectedChildren, result.Children.Count());
        }

        [Test]
        public void ParseDocumentSource_NestedContent_PlacesLineBreaksCorrectly()
        {
            string template = "%p Line 1\n%p\n  Line 2\n%p Line 3";
            var result = _parser.ParseDocumentSource(template);

            var children = result.Children.ToList();

            Assert.That(children[1].Content, Is.EqualTo("\n"));
            Assert.That(children[2].Children.First().Content, Is.EqualTo("\n"));
            Assert.That(children[2].Children.Count(), Is.EqualTo(2));
            Assert.That(children[3].Content, Is.EqualTo("\n"));
        }

        [Test]
        public void ParseHamlFile_UnknownRuleType_ThrowsUnknownRuleException()
        {
            var fakeLine = new HamlLineFake("") {HamlRule = HamlRuleEnum.Unknown};

            var file = new HamlFile();
            file.AddLine(fakeLine);
            Assert.Throws<HamlUnknownRuleException>(() => _parser.ParseHamlFile(file));           
        }

        class HamlLineFake : HamlLine
        {
            public HamlLineFake(string line) : base(line, 0) { }

            public new HamlRuleEnum HamlRule
            {
                set { base.HamlRule = value;  }
            }
        }

    }
}
