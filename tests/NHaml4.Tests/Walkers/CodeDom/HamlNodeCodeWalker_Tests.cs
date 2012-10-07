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
        public void Walk_ValidNodeWithNoChildren_CallsAppendCodeSnippetMethodWithFalse()
        {
            string codeSnippet = "int c = 1";
            var node = new HamlNodeCode(new HamlLine(-1, codeSnippet, "", NHaml4.Parser.HamlRuleEnum.Code));

            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.AppendCodeSnippet(codeSnippet, false));
        }

        [Test]
        public void Walk_ValidNodeWithChildren_CallsAppendCodeSnippetMethodWithTrue()
        {
            string codeSnippet = "int c = 1";
            var node = new HamlNodeCode(new HamlLine(-1, codeSnippet, "", NHaml4.Parser.HamlRuleEnum.Code));
            node.AddChild(new HamlNodeTextContainer(-1, ""));

            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.AppendCodeSnippet(codeSnippet, true));
        }

        [Test]
        public void Walk_ValidNodeWithChildren_CallsRenderEndBlock()
        {
            string codeSnippet = "int c = 1";
            var node = new HamlNodeCode(new HamlLine(-1, codeSnippet, "", NHaml4.Parser.HamlRuleEnum.Code));
            node.AddChild(new HamlNodeTextContainer(-1, ""));

            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.RenderEndBlock());
        }

        [Test]
        public void Walk_ChildNode_DoesNotThrowInvalidChildNodeException()
        {
            var node = new HamlNodeCode(new HamlLine(-1, "1+1", "", NHaml4.Parser.HamlRuleEnum.Code));
            node.AddChild(new HamlNodeTextContainer(0, ""));

            Assert.DoesNotThrow(() => _walker.Walk(node));
        }

        [Test]
        public void Walk_ConditionalWithChildNode_RendersChildNode()
        {
            const string dummyText = "Hello";
            var node = new HamlNodeCode(new HamlLine(-1, "if (true)", "", NHaml4.Parser.HamlRuleEnum.Code));
            node.AddChild(new HamlNodeTextContainer(0, dummyText));
            var classBuilder = new ClassBuilderMock();

            var walker = new HamlNodeCodeWalker(classBuilder, new HamlHtmlOptions());
            walker.Walk(node);

            Assert.That(classBuilder.Build(""), Is.EqualTo(dummyText));
        }

        [Test]
        public void Walk_ValidNode_DoesNotGenerateContent()
        {
            var node = new HamlNodeCode(new HamlLine(-1, "1+1", "", NHaml4.Parser.HamlRuleEnum.Code));
            var classBuilder = new ClassBuilderMock();
            var walker = new HamlNodeCodeWalker(classBuilder, new HamlHtmlOptions());
            walker.Walk(node);

            Assert.That(classBuilder.Build(""), Is.EqualTo(""));
        }

        [Test]
        public void AppendInnerTagNewLine_ValidNode_DoesNotAppendNewLine()
        {
            var node = new HamlNodeCode(new HamlLine(-1, "1+1", "", NHaml4.Parser.HamlRuleEnum.Code));

            node.AppendInnerTagNewLine();

            Assert.That(node.Children.Any(), Is.False);
        }

        [Test]
        public void AppendPostTagNewLine_ValidNode_DoesNotAppendNewLine()
        {
            var node = new HamlNodeCode(new HamlLine(-1, "1+1", "", NHaml4.Parser.HamlRuleEnum.Code));
            var childNode = new HamlNodeCode(new HamlLine(-1, "1+1", "", NHaml4.Parser.HamlRuleEnum.Code));

            node.AppendPostTagNewLine(childNode, -1);

            Assert.That(node.Children.Any(), Is.False);
        }
    }
}
