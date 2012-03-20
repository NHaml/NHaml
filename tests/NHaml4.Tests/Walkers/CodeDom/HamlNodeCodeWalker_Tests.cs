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

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeCodeWalker_Tests
    {
        private Mock<ITemplateClassBuilder> _classBuilderMock;
        private HamlNodeCodeWalker _walker;

        [SetUp]
        public void Setup()
        {
            _classBuilderMock = new Mock<ITemplateClassBuilder>();
            _walker = new HamlNodeCodeWalker(_classBuilderMock.Object, new HamlHtmlOptions());
        }

        [Test]
        public void Walk_InvalidNodeType_ThrowsInvalidCastException()
        {
            var node = new HamlNodeTextContainer(0, "");

            Assert.Throws<InvalidCastException>(() => _walker.Walk(node));
        }

        [Test]
        public void Walk_ValidNode_CallsAppendCodeSnippetMethod()
        {
            string codeSnippet = "int c = 1";
            var node = new HamlNodeCode(new HamlLine(codeSnippet, -1));

            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.AppendCodeSnippet(codeSnippet));
        }

        [Test]
        public void Walk_ChildNode_DoesNotThrowInvalidChildNodeException()
        {
            var node = new HamlNodeCode(new HamlLine("1+1", -1));
            node.AddChild(new HamlNodeTextContainer(0, ""));

            Assert.DoesNotThrow(() => _walker.Walk(node));
        }

        [Test]
        public void Walk_ConditionalWithChildNode_RendersChildNode()
        {
            const string dummyText = "Hello";
            var node = new HamlNodeCode(new HamlLine("if (true)", -1));
            node.AddChild(new HamlNodeTextContainer(0, dummyText));
            var classBuilder = new ClassBuilderMock();

            var walker = new HamlNodeCodeWalker(classBuilder, new HamlHtmlOptions());
            walker.Walk(node);

            Assert.That(classBuilder.Build(""), Is.EqualTo(dummyText));
        }

        [Test]
        public void Walk_ValidNode_DoesNotGenerateContent()
        {
            var node = new HamlNodeCode(new HamlLine("1+1", -1));
            var classBuilder = new ClassBuilderMock();
            var walker = new HamlNodeCodeWalker(classBuilder, new HamlHtmlOptions());
            walker.Walk(node);

            Assert.That(classBuilder.Build(""), Is.EqualTo(""));
        }

        [Test]
        public void AppendInnerTagNewLine_ValidNode_DoesNotAppendNewLine()
        {
            var node = new HamlNodeCode(new HamlLine("1+1", -1));

            node.AppendInnerTagNewLine();

            Assert.That(node.Children.Any(), Is.False);
        }

        [Test]
        public void AppendPostTagNewLine_ValidNode_DoesNotAppendNewLine()
        {
            var node = new HamlNodeCode(new HamlLine("1+1", -1));

            node.AppendPostTagNewLine(-1);

            Assert.That(node.Children.Any(), Is.False);
        }
    }
}
