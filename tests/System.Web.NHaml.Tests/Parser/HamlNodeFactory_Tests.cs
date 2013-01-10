using System.Web.NHaml.IO;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Exceptions;
using System.Web.NHaml.Parser.Rules;
using NUnit.Framework;
using System;

namespace NHaml.Tests.Parser
{
    [TestFixture]
    public class HamlNodeFactory_Tests
    {
        [Test]
        [TestCase(HamlRuleEnum.PlainText, typeof(HamlNodeTextContainer))]
        [TestCase(HamlRuleEnum.Tag, typeof(HamlNodeTag))]
        [TestCase(HamlRuleEnum.HamlComment, typeof(HamlNodeHamlComment))]
        [TestCase(HamlRuleEnum.HtmlComment, typeof(HamlNodeHtmlComment))]
        [TestCase(HamlRuleEnum.Evaluation, typeof(HamlNodeEval))]
        public void GetHamlNode_DifferentHamlLineTypes_ReturnsCorrectHamlNode(HamlRuleEnum rule, Type nodeType)
        {
            var line = new HamlLine("Blah", rule, "", 0);
            var result = HamlNodeFactory.GetHamlNode(line);
            Assert.That(result, Is.InstanceOf(nodeType));
        }

        [Test]
        [TestCase(HamlRuleEnum.DivClass, typeof(HamlNodeTag))]
        [TestCase(HamlRuleEnum.DivId, typeof(HamlNodeTag))]
        public void GetHamlNode_TagSubTypes_ThrowsHamlUnknownRuleException(HamlRuleEnum rule, Type nodeType)
        {
            var line = new HamlLine("Blah", rule, "", 0);
            Assert.Throws<HamlUnknownRuleException>(() => HamlNodeFactory.GetHamlNode(line));
        }
    }
}
