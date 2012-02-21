using NUnit.Framework;
using NHaml4.Parser.Rules;
using NHaml4.IO;
using System;
using NHaml4.Parser.Exceptions;
namespace NHaml4.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeTextContainer_Tests
    {
        [Test]
        [TestCase("Test", false)]
        [TestCase("   ", true)]
        [TestCase("\n", true)]
        [TestCase("\t", true)]
        public void IsWhitespace_ReturnsCorrectResult(string whiteSpace, bool expectedResult)
        {
            var node = new HamlNodeTextContainer(new HamlLine(whiteSpace, 0));
            Assert.That(node.IsWhitespace(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("Text", typeof(HamlNodeTextLiteral))]
        [TestCase("#{Variable}", typeof(HamlNodeTextVariable))]
        [TestCase("\\#{Variable}", typeof(HamlNodeTextLiteral))]
        [TestCase("#{V}", typeof(HamlNodeTextVariable))]
        [TestCase("#{}", typeof(HamlNodeTextLiteral))]
        [TestCase("}", typeof(HamlNodeTextLiteral))]
        public void Children_FirstChildIsOfCorrectType(string line, Type expectedType)
        {
            var node = new HamlNodeTextContainer(new HamlLine(line, 0));
            Assert.That(node.Children[0], Is.InstanceOf(expectedType));
            Assert.That(node.Children.Count, Is.EqualTo(1));
        }

        public void Children_EmptyString_NoChildren()
        {
            var node = new HamlNodeTextContainer(new HamlLine("", 0));
            Assert.That(node.Children.Count, Is.EqualTo(0));
        }

        [Test]
        //[TestCase("Text#{Variable}", typeof(HamlNodeTextLiteral), typeof(HamlNodeTextVariable))]
        //[TestCase("#{Variable}Text", typeof(HamlNodeTextVariable), typeof(HamlNodeTextLiteral))]
        //[TestCase("#{Variable1}#{Variable}", typeof(HamlNodeTextVariable), typeof(HamlNodeTextVariable))]
        [TestCase("\\\\#{Variable1}", typeof(HamlNodeTextLiteral), typeof(HamlNodeTextVariable))]
        public void Children_MultipleFragments_ChildrenAreOfCorrectType(string line, Type node1Type, Type node2Type)
        {
            var node = new HamlNodeTextContainer(new HamlLine(line, 0));
            Assert.That(node.Children[0], Is.InstanceOf(node1Type));
            Assert.That(node.Children[1], Is.InstanceOf(node2Type));
        }

        [Test]
        public void Children_EscapedContent_RemovesEscapeCharacter()
        {
            var node = new HamlNodeTextContainer(new HamlLine("\\#{variable}", 0));
            Assert.That(node.Children[0].Content, Is.EqualTo("#{variable}"));
        }

        [Test]
        public void Children_IncompleteVariableReference_ThrowsException()
        {
            var line = new HamlLine("#{variable", 0);
            Assert.Throws<HamlMalformedVariableException>(() => new HamlNodeTextContainer(line));
        }
    }
}
