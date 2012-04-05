using NUnit.Framework;
using System;
using NHaml4.IO;
using NHaml4.Parser;
using NHaml4.Parser.Rules;

namespace NHaml4.Tests.Parser
{
    [TestFixture]
    public class HamlNodeFactory_Tests
    {
        [Test]
        [TestCase("Test", typeof(HamlNodeTextContainer))]
        [TestCase("%Test", typeof(HamlNodeTag))]
        [TestCase(".Test", typeof(HamlNodeTag))]
        [TestCase("#Test", typeof(HamlNodeTag))]
        [TestCase("-#Test", typeof(HamlNodeHamlComment))]
        [TestCase("/Test", typeof(HamlNodeHtmlComment))]
        [TestCase("=Test", typeof(HamlNodeEval))]
        [TestCase("-Test", typeof(HamlNodeCode))]
        [TestCase("!!!Test", typeof(HamlNodeDocType))]
        public void GetHamlNode_DifferentHamlLineTypes_ReturnsCorrectHamlNode(string hamlLine, Type nodeType)
        {
            var line = new HamlLine(hamlLine, 0);
            var result = HamlNodeFactory.GetHamlNode(line);
            Assert.That(result, Is.InstanceOf(nodeType));
        }
    }
}
