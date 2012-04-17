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
    class HamlRuleFactory_Tests
    {
        [Test]
        [TestCase("", HamlRuleEnum.PlainText, Description = "Empty string")]
        [TestCase(" ", HamlRuleEnum.PlainText, Description = "Single space")]
        [TestCase("%", HamlRuleEnum.Tag, Description = "Plain tag")]
        [TestCase(".className", HamlRuleEnum.DivClass, Description = "Plain tag")]
        [TestCase("#id", HamlRuleEnum.DivId, Description = "Plain tag")]
        [TestCase("%Tag", HamlRuleEnum.Tag, Description = "Plain tag")]
        [TestCase("/Tag", HamlRuleEnum.HtmlComment, Description = "HTML Comment")]
        [TestCase("-#Tag", HamlRuleEnum.HamlComment, Description = "Haml Comment")]
        [TestCase("!!!Tag", HamlRuleEnum.DocType, Description = "DocType")]
        [TestCase("=Tag", HamlRuleEnum.Evaluation, Description = "DocType")]
        [TestCase("-Statement", HamlRuleEnum.Code, Description = "Haml Comment")]
        [TestCase("\\%EscapedTag", HamlRuleEnum.PlainText, Description = "Escaped Tag")]
        [TestCase("_Partial", HamlRuleEnum.Partial, Description = "Partial Reference")]
        public void Constructor_CalculatesRuleTypeCorrectly(string testString, HamlRuleEnum expectedRule)
        {
            var rule = HamlRuleFactory.ParseHamlRule(ref testString);
            Assert.AreEqual(expectedRule, rule);
        }

        [Test]
        [TestCase("", "", Description = "Empty string")]
        [TestCase("%", "", Description = "Plain tag")]
        [TestCase(".className", ".className", Description = "Implicit div via class")]
        [TestCase("#id", "#id", Description = "Implicit div via id")]
        [TestCase("%Tag", "Tag", Description = "Plain tag")]
        [TestCase("/Tag", "Tag", Description = "HTML Comment")]
        [TestCase("-#Tag", "Tag", Description = "Haml Comment")]
        [TestCase("!!!Tag", "Tag", Description = "DocType")]
        [TestCase("\\%EscapedTag", "%EscapedTag", Description = "Escaped Tag")]
        public void Constructor_ExtractsContentCorrectly(string testString, string expectedContent)
        {
            var rule = HamlRuleFactory.ParseHamlRule(ref testString);
            Assert.AreEqual(expectedContent, testString);
        }
    }
}
