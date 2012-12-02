using NUnit.Framework;
using NHaml.IO;
using NHaml.Parser;

namespace NHaml.Tests.IO
{
    [TestFixture]
    public class HamlLine_Tests
    {
        [Test]
        [TestCase("", "", 0, Description = "Empty string")]
        [TestCase(" ", "", 0, Description = "Single space")]
        [TestCase("", "Test", 0, Description = "No space followed by plain text")]
        [TestCase(" ", "Test", 1, Description = "Space followed by plain text")]
        [TestCase("\t", "Test", 2, Description = "Tab followed by plain text")]
        [TestCase(" \t", "Test", 3, Description = "Space + Tab followed by plain text")]
        [TestCase("\t ", "Test", 3, Description = "Tab + Space followed by plain text")]
        public void Constructor_CalculatesIndentCountCorrectly(string indent, string content, int expectedIndent)
        {
            var line = new HamlLine(content, HamlRuleEnum.PlainText, indent, 0);
            Assert.AreEqual(expectedIndent, line.IndentCount);
        }
    }
}
