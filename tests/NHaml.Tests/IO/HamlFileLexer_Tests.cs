using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml.IO;
using System.IO;
using NHaml4.IO;

namespace NHaml.Tests.IO
{
    [TestFixture]
    public class HamlFileLexer_Tests
    {
        [Test]
        public void Read_NormalUse_ReturnsHamlFile()
        {
            var textReader = new StringReader("");
            var result = new HamlFileLexer().Read(textReader);
            Assert.IsInstanceOf<HamlFile>(result);
        }

        [Test]
        [TestCase("", 0, Description = "Empty Line")]
        [TestCase("Line", 1, Description = "Single Line")]
        [TestCase("Line1\n", 1, Description = "Single Line Followed By Line Break")]
        [TestCase("Line1\nLine2", 2, Description = "Two Lines")]
        [TestCase("Line1\n\nLine2", 3, Description = "Two Lines Separated With Blank Line")]
        public void Read_ReturnsHamlFileWithCorrectLineCount(string template, int expectedLineCount)
        {
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader);
            Assert.AreEqual(expectedLineCount, result.LineCount);
        }

        [Test]
        [TestCase("Test", 0, Description = "No indent")]
        [TestCase("  Test", 2, Description = "Two Spaces")]
        [TestCase("\tTest", 2, Description = "One Tab")]
        [TestCase("\t  Test", 4, Description = "Tab And Space")]
        [TestCase("  \tTest", 4, Description = "Space And Tab ")]
        public void Read_ReturnsLineWithCorrectIndent(string template, int expectedIndentCount)
        {
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader);
            Assert.AreEqual(expectedIndentCount, result.CurrentLine.IndentCount);
        }

        [Test]
        public void Read_SingleLineTemplate_MoveNextReturnsNullCurrentLine()
        {
            string template = "test";
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader);
            result.MoveNext();
            Assert.IsNull(result.CurrentLine);
        }

        [Test]
        public void Read_MultiLineTemplate_MoveNextReturnsSecondCurrentLine()
        {
            string template = "test\ntest2";
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader);
            result.MoveNext();
            Assert.AreEqual("test2", result.CurrentLine.Content);
        }

    }
}
