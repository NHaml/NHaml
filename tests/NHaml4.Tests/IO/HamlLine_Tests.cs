using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.IO;
using NHaml4.Parser;

namespace NHaml4.Tests.IO
{
    [TestFixture]
    public class HamlLine_Tests
    {
        [Test]
        [TestCase("", 0, Description = "Empty string")]
        [TestCase(" ", 0, Description = "Single space")]
        [TestCase("Test", 0, Description = "No space followed by plain text")]
        [TestCase(" Test", 1, Description = "Space followed by plain text")]
        [TestCase("\tTest", 2, Description = "Tab followed by plain text")]
        [TestCase(" \tTest", 3, Description = "Space + Tab followed by plain text")]
        [TestCase("\t Test", 3, Description = "Tab + Space followed by plain text")]
        public void Constructor_CalculatesIndentCountCorrectly(string testString, int expectedIndent)
        {
            var line = new HamlLine(testString, 0);
            Assert.AreEqual(expectedIndent, line.IndentCount);
        }

        [Test]
        [TestCase("", "", Description = "Empty string")]
        [TestCase(" ", " ", Description = "Single space")]
        [TestCase("Test", "", Description = "No space followed by plain text")]
        [TestCase(" Test", " ", Description = "Space followed by plain text")]
        [TestCase("\tTest", "\t", Description = "Tab followed by plain text")]
        [TestCase(" \tTest", " \t", Description = "Space + Tab followed by plain text")]
        [TestCase("\t Test", "\t ", Description = "Tab + Space followed by plain text")]
        public void Constructor_CalculatesIndentCorrectly(string testString, string expectedIndent)
        {
            var line = new HamlLine(testString, 0);
            Assert.AreEqual(expectedIndent, line.Indent);
        }

        [Test]
        [TestCase("", HamlRuleEnum.PlainText, Description = "Empty string")]
        [TestCase(" ", HamlRuleEnum.PlainText, Description = "Single space")]
        [TestCase("%", HamlRuleEnum.Tag, Description = "Plain tag")]
        [TestCase(".className", HamlRuleEnum.Tag, Description = "Plain tag")]
        [TestCase("#id", HamlRuleEnum.Tag, Description = "Plain tag")]
        [TestCase("%Tag", HamlRuleEnum.Tag, Description = "Plain tag")]
        [TestCase("/Tag", HamlRuleEnum.HtmlComment, Description = "HTML Comment")]
        [TestCase("-#Tag", HamlRuleEnum.HamlComment, Description = "Haml Comment")]
        [TestCase("!!!Tag", HamlRuleEnum.DocType, Description = "DocType")]
        [TestCase("=Tag", HamlRuleEnum.Evaluation, Description = "DocType")]
        public void Constructor_CalculatesRuleTypeCorrectly(string testString, HamlRuleEnum expectedRule)
        {
            var line = new HamlLine(testString, 0);
            Assert.AreEqual(expectedRule, line.HamlRule);
        }

        [Test]
        [TestCase("", "", Description = "Empty string")]
        [TestCase(" ", "", Description = "Single space")]
        [TestCase("%", "", Description = "Plain tag")]
        [TestCase(".className", ".className", Description = "Plain tag")]
        [TestCase("#id", "#id", Description = "Plain tag")]
        [TestCase("%Tag", "Tag", Description = "Plain tag")]
        [TestCase(" /Tag", "Tag", Description = "HTML Comment")]
        [TestCase("  -#Tag", "Tag", Description = "Haml Comment")]
        [TestCase("\t\t!!!Tag", "Tag", Description = "DocType")]
        public void Constructor_ExtractsContentCorrectly(string testString, string expectedContent)
        {
            var line = new HamlLine(testString, 0);
            Assert.AreEqual(expectedContent, line.Content);
        }
    }
}
