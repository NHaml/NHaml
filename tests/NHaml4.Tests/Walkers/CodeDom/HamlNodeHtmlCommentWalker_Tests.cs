using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml4.Compilers;
using Moq;
using NHaml4.IO;
using NHaml4.Parser.Rules;
using NHaml4.Tests.Mocks;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeHtmlCommentWalker_Tests
    {
        private class BogusHamlNode : HamlNode
        {
            public BogusHamlNode() : base(0, "") { }

            public override bool IsContentGeneratingTag
            {
                get { return true; }
            }
        }

        ClassBuilderMock _classBuilderMock;
        private HamlNodeHtmlCommentWalker _walker;
        private HamlHtmlOptions _hamlOptions;

        [SetUp]
        public void SetUp()
        {
            _classBuilderMock = new ClassBuilderMock();
            _hamlOptions = new HamlHtmlOptions();
            _walker = new HamlNodeHtmlCommentWalker(_classBuilderMock, _hamlOptions);
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
            var node = new HamlNodeHtmlComment(new HamlLine(comment, 0));

            // Act
            _walker.Walk(node);

            // Assert
            Assert.That(_classBuilderMock.Build(""), Is.EqualTo("<!--" + comment + " -->"));
        }

        [Test]
        public void Walk_NestedTags_AppendsCorrectTags()
        {
            // Arrange
            HamlLine nestedText = new HamlLine("  Hello world", 0);
            var tagNode = new HamlNodeHtmlComment(new HamlLine("", 0));
            tagNode.AddChild(new HamlNodeTextContainer(nestedText));

            // Act
            _walker.Walk(tagNode);

            // Assert
            string expectedComment = "<!--" + nestedText.Indent + nestedText.Content + " -->";
            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedComment));
        }
    }
}
