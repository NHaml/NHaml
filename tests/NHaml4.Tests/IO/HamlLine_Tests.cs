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
        [TestCase("", "", 0, Description = "Empty string")]
        [TestCase(" ", "", 0, Description = "Single space")]
        [TestCase("", "Test", 0, Description = "No space followed by plain text")]
        [TestCase(" ", "Test", 1, Description = "Space followed by plain text")]
        [TestCase("\t", "Test", 2, Description = "Tab followed by plain text")]
        [TestCase(" \t", "Test", 3, Description = "Space + Tab followed by plain text")]
        [TestCase("\t ", "Test", 3, Description = "Tab + Space followed by plain text")]
        public void Constructor_CalculatesIndentCountCorrectly(string indent, string content, int expectedIndent)
        {
            var line = new HamlLine(0, content, indent, HamlRuleEnum.PlainText);
            Assert.AreEqual(expectedIndent, line.IndentCount);
        }
    }
}
