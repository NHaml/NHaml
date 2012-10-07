using NUnit.Framework;
using Moq;
using NHaml4.Compilers;
using NHaml4.Walkers.CodeDom;
using NHaml4.Parser.Rules;
using NHaml4.IO;
using NHaml4.Walkers.Exceptions;
using System;
using NHaml4.Tests.Mocks;
using System.Linq;
using NHaml4.TemplateBase;

namespace NHaml4.Tests.Walkers.CodeDom
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
            var node = new HamlNodeDocType(new HamlLine(-1, "", "", NHaml4.Parser.HamlRuleEnum.DocType));

            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.AppendDocType(""));
        }

        [Test]
        public void Walk_ValidNodeWithChildren_Throws()
        {
            var node = new HamlNodeDocType(new HamlLine(-1, "", "", NHaml4.Parser.HamlRuleEnum.DocType));
            node.AddChild(new HamlNodeTextContainer(-1, ""));

            Assert.Throws<HamlInvalidChildNodeException>(() => _walker.Walk(node));
        }
    }
}
