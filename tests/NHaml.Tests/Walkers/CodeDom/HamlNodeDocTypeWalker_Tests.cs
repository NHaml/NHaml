using NUnit.Framework;
using Moq;
using NHaml.Compilers;
using NHaml.Walkers.CodeDom;
using NHaml.Parser.Rules;
using NHaml.IO;
using NHaml.Walkers.Exceptions;
using System;

namespace NHaml.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeDocTypeWalker_Tests
    {
        private Mock<ITemplateClassBuilder> _classBuilderMock;
        private HamlNodeDocTypeWalker _walker;

        [SetUp]
        public void Setup()
        {
            _classBuilderMock = new Mock<ITemplateClassBuilder>();
            _walker = new HamlNodeDocTypeWalker(_classBuilderMock.Object, new HamlHtmlOptions());
        }

        [Test]
        public void Walk_InvalidNodeType_ThrowsInvalidCastException()
        {
            var node = new HamlNodeTextContainer(0, "");

            Assert.Throws<InvalidCastException>(() => _walker.Walk(node));
        }

        [Test]
        public void Walk_ValidNodeWithNoChildren_AppendsDocType()
        {
            var node = new HamlNodeDocType(new HamlLine("", NHaml.Parser.HamlRuleEnum.DocType, "", -1));

            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.AppendDocType(""));
        }

        [Test]
        public void Walk_ValidNodeWithChildren_Throws()
        {
            var node = new HamlNodeDocType(new HamlLine("", NHaml.Parser.HamlRuleEnum.DocType, "", -1));
            node.AddChild(new HamlNodeTextContainer(-1, ""));

            Assert.Throws<HamlInvalidChildNodeException>(() => _walker.Walk(node));
        }
    }
}
