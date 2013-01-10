using System.Web.NHaml.Compilers;
using System.Web.NHaml.IO;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.Walkers.CodeDom;
using System.Web.NHaml.Walkers.Exceptions;
using NUnit.Framework;
using Moq;
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
            var node = new HamlNodeDocType(new HamlLine("", HamlRuleEnum.DocType, "", -1));

            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.AppendDocType(""));
        }

        [Test]
        public void Walk_ValidNodeWithChildren_Throws()
        {
            var node = new HamlNodeDocType(new HamlLine("", HamlRuleEnum.DocType, "", -1));
            node.AddChild(new HamlNodeTextContainer(-1, ""));

            Assert.Throws<HamlInvalidChildNodeException>(() => _walker.Walk(node));
        }
    }
}
