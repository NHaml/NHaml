using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml.Walkers.CodeDom;
using NHaml.Parser.Rules;

namespace NHaml.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlWalkerFactory_Tests
    {
        [Test]
        [TestCase(typeof(HamlNodeTextContainer), typeof(HamlNodeTextContainerWalker))]
        [TestCase(typeof(HamlNodeTag), typeof(HamlNodeTagWalker))]
        [TestCase(typeof(HamlNodeHtmlComment), typeof(HamlNodeHtmlCommentWalker))]
        [TestCase(typeof(HamlNodeHamlComment), typeof(HamlNodeHamlCommentWalker))]
        [TestCase(typeof(HamlNodeEval), typeof(HamlNodeEvalWalker))]
        [TestCase(typeof(HamlNodeTextLiteral), typeof(HamlNodeTextLiteralWalker))]
        [TestCase(typeof(HamlNodeTextVariable), typeof(HamlNodeTextVariableWalker))]
        [TestCase(typeof(HamlNodeCode), typeof(HamlNodeCodeWalker))]
        [TestCase(typeof(HamlNodeDocType), typeof(HamlNodeDocTypeWalker))]
        [TestCase(typeof(HamlNodePartial), typeof(HamlPartialWalker))]
        public void GetNodeWalker_VariousNodeTypes_ReturnsCorrectWalker(Type nodeType, Type expectedWalkerType)
        {
            var walker = HamlWalkerFactory.GetNodeWalker(nodeType, -1, new Mocks.ClassBuilderMock(), new HamlHtmlOptions());
            Assert.That(walker, Is.InstanceOf(expectedWalkerType));
        }
    }
}
