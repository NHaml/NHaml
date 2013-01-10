using System.Web.NHaml.IO;
using System.Web.NHaml.Parser;
using NUnit.Framework;

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

        [Test]
        [TestCase(" ", "", HamlRuleEnum.PlainText, 0)]
        [TestCase(" ", "Test", HamlRuleEnum.PlainText, 1)]
        [TestCase(" ", "", HamlRuleEnum.Partial, 1)]
        public void Constructor_MaintainsIndentForNonTextNodes(string indent, string content, HamlRuleEnum rule, int expectedIndent)
        {
            var line = new HamlLine(content, rule, indent, 0);
            Assert.AreEqual(expectedIndent, line.IndentCount);
        }
    }
}
