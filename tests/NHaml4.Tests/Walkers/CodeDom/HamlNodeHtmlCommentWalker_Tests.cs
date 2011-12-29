using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml4.Compilers;
using Moq;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeHtmlCommentWalker_Tests
    {
        private class BogusHamlNode : HamlNode
        {

        }
        Mock<ITemplateClassBuilder> _classBuilderMock;
        private HamlNodeHtmlCommentWalker _walker;
        private HamlOptions _hamlOptions;

        [SetUp]
        public void SetUp()
        {
            _classBuilderMock = new Mock<ITemplateClassBuilder>();
            _hamlOptions = new HamlOptions();
            _walker = new HamlNodeHtmlCommentWalker(_classBuilderMock.Object, _hamlOptions);
        }

        [Test]
        public void Walk_NodeIsWrongType_ThrowsException()
        {
            var node = new BogusHamlNode();
            Assert.Throws<InvalidCastException>(() => _walker.Walk(node));
        }

        [Test]
        public void Walk_ValidNode_AppendsCorrectOutput()
        {
            // Arrange
            string comment = "Comment";
            var node = new HamlNodeHtmlComment(comment);

            // Act
            _walker.Walk(node);

            // Assert
            _classBuilderMock.Verify(x => x.Append("<!--" + comment));
            _classBuilderMock.Verify(x => x.Append(" -->"));
        }
    }
}
