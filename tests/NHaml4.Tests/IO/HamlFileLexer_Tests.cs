using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NHaml4.IO;

namespace NHaml4.Tests.IO
{
    [TestFixture]
    public class HamlFileLexer_Tests
    {
        [Test]
        public void Read_NormalUse_ReturnsHamlFile()
        {
            var textReader = new StringReader("");
            var result = new HamlFileLexer().Read(textReader, "");
            Assert.IsInstanceOf<HamlFile>(result);
        }

        [Test]
        public void Read_NullArgument_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new HamlFileLexer().Read(null, ""));
        }

        [Test]
        [TestCase("", 0, Description = "Empty Line")]
        [TestCase("Line", 1, Description = "Single Line")]
        [TestCase("Line1\n", 2, Description = "Single Line Followed By Line Break")]
        [TestCase("Line1\nLine2", 2, Description = "Two Lines")]
        [TestCase("Line1\n\nLine2", 3, Description = "Two Lines Separated With Blank Line")]
        public void Read_ReturnsHamlFileWithCorrectLineCount(string template, int expectedLineCount)
        {
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader, "");
            Assert.AreEqual(expectedLineCount, result.LineCount);
        }

        [TestCase("Line1\n", 2, "Line1", "", Description = "Single Line Followed By Line Break")]
        [TestCase("Line1\nLine2", 2, "Line1", "Line2", Description = "Single Line Followed By Line Break")]
        [TestCase("Line1\rLine2", 2, "Line1", "Line2", Description = "Single Line Followed By Line Break")]
        [TestCase("Line1\r\nLine2", 2, "Line1", "Line2", Description = "Single Line Followed By Line Break")]
        [TestCase("Line1\n\rLine2", 3, "Line1", "", Description = "Single Line Followed By Line Break")]
        public void Read_HandlesNonStandardLineBreaksCorrectly(string template, int expectedLineCount, string expectedLine1, string expectedLine2)
        {
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader, "");

            // Assert
            Assert.AreEqual(expectedLineCount, result.LineCount);
            Assert.AreEqual(expectedLine1, result.CurrentLine.Content);
            if (!result.EndOfFile)
            {
                result.MoveNext();
                Assert.AreEqual(expectedLine2, result.CurrentLine.Content);
            }
        }

        public void Read_DeterminesLineNumbersCorrectly()
        {
            var textReader = new StringReader("Line1\nLine2\r\nLine3\n\rLine4");
            var result = new HamlFileLexer().Read(textReader, "");

            // Assert
            for (int c = 1; c < 5; c++)
            {
                Assert.That(result.CurrentLine.SourceFileLineNo, Is.EqualTo(c));
                result.MoveNext();
            }
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
            var result = new HamlFileLexer().Read(textReader, "");
            Assert.AreEqual(expectedIndentCount, result.CurrentLine.IndentCount);
        }

        [Test]
        public void Read_SingleLineTemplate_MoveNextReturnsNullCurrentLine()
        {
            string template = "test";
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader, "");
            result.MoveNext();
            Assert.IsNull(result.CurrentLine);
        }

        [Test]
        public void Read_MultiLineTemplate_MoveNextReturnsSecondCurrentLine()
        {
            string template = "test\ntest2";
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader, "");
            result.MoveNext();
            Assert.AreEqual("test2", result.CurrentLine.Content);
        }

        [Test]
        [TestCase("%p(a='b'\nc='d')content", "p(a='b' c='d')", "content")]
        [TestCase("%p(a=')b'\nc='d')content", "p(a=')b' c='d')", "content")]
        [TestCase("%p(a=\")b\"\nc='d')content", "p(a=\")b\" c='d')", "content")]
        [TestCase("%p{a='b'\nc='d'}content", "p{a='b' c='d'}", "content")]
        [TestCase("%p{a='}b'\nc='d'}content", "p{a='}b' c='d'}", "content")]
        [TestCase("%p{a=\"}b\"\nc='d'}content", "p{a=\"}b\" c='d'}", "content")]
        public void Read_SplitLineTag_ReturnsSingleLine(string template, string expectedTag, string expectedContent)
        {
            var textReader = new StringReader(template);
            var result = new HamlFileLexer().Read(textReader, "");
            Assert.That(result.CurrentLine.Content, Is.EqualTo(expectedTag));
            result.MoveNext();
            Assert.That(result.CurrentLine.Content, Is.EqualTo(expectedContent));
        }

        public void Read_FileName_ResultContainsFileName()
        {
            const string fileName = "testFileName";
            var textReader = new StringReader("");
            var result = new HamlFileLexer().Read(textReader, fileName);

            Assert.That(result.FileName, Is.EqualTo(fileName));
        }

    }
}
