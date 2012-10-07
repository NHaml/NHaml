using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml4.Compilers;
using Moq;
using NHaml4.Parser.Rules;
using NHaml4.IO;
using NHaml4.Tests.Mocks;
using NHaml4.Tests.Builders;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTextContainerWalker_Tests
    {
        private ClassBuilderMock _mockClassBuilder;
        private HamlNodeTextContainerWalker _walker;

        private class BogusHamlNode : HamlNode
        {
            public BogusHamlNode() : base(0, "") { }

            protected override bool IsContentGeneratingTag
            {
                get { return true; }
            }
        }

        [SetUp]
        public void SetUp()
        {
            _mockClassBuilder = new ClassBuilderMock();
            _walker = new HamlNodeTextContainerWalker(_mockClassBuilder, new HamlHtmlOptions());
        }

        [Test]
        public void Walk_NodeIsWrongType_ThrowsException()
        {
            var node = new BogusHamlNode();
            Assert.Throws<InvalidCastException>(() => _walker.Walk(node));
        }

        [Test]
        public void Walk_IndentedNode_WritesIndent()
        {
            const string indent = "  ";
            var node = new HamlNodeTextContainer(new HamlLine(0, "Content", indent, HamlRuleEnum.PlainText));

            _walker.Walk(node);

            Assert.That(_mockClassBuilder.Build(""), Is.StringStarting(indent));
        }

        [Test]
        [TestCase("   ")]
        [TestCase("\n\t   ")]
        public void Walk_PreviousTagHasSurroundingWhitespaceRemoved_RendersTag(string whiteSpace)
        {
            var node = HamlDocumentBuilder.Create("",
                new HamlNodeTag(new HamlLine(-1, "p>", "", HamlRuleEnum.Tag)),
                new HamlNodeTextContainer(new HamlLine(-1, "", whiteSpace, HamlRuleEnum.PlainText)));

            new HamlDocumentWalker(_mockClassBuilder).Walk(node);

            Assert.That(_mockClassBuilder.Build(""), Is.EqualTo("<p></p>"));
        }

        [Test]
        public void Walk_MultipleWhitespaceWithPreviousTagSurroundingWhitespaceRemoved_RendersTag()
        {
            var node = HamlDocumentBuilder.Create("",
                new HamlNodeTag(new HamlLine(-1, "p>", "", HamlRuleEnum.Tag)),
                new HamlNodeTextContainer(new HamlLine(-1, "", "   ", HamlRuleEnum.PlainText)),
                new HamlNodeTextContainer(new HamlLine(-1, "", "   ", HamlRuleEnum.PlainText)),
                new HamlNodeTextContainer(new HamlLine(-1, "", "   ", HamlRuleEnum.PlainText)));

            new HamlDocumentWalker(_mockClassBuilder).Walk(node);

            Assert.That(_mockClassBuilder.Build(""), Is.EqualTo("<p></p>"));
        }

        [Test]
        [TestCase("   ")]
        [TestCase("\n\t   ")]
        public void Walk_NextTagHasSurroundingWhitespaceRemoved_RendersTag(string whiteSpace)
        {
            var node = HamlDocumentBuilder.Create("",
                new HamlNodeTextContainer(new HamlLine(-1, "", whiteSpace, HamlRuleEnum.PlainText)),
                new HamlNodeTag(new HamlLine(-1, "p>", "", HamlRuleEnum.Tag)));

            new HamlDocumentWalker(_mockClassBuilder).Walk(node);

            Assert.That(_mockClassBuilder.Build(""), Is.EqualTo("<p></p>"));
        }

        [Test]
        public void Walk_MultipleWhitespaceWithNextTagSurroundingWhitespaceRemoved_RendersTag()
        {
            var node = HamlDocumentBuilder.Create("",
                new HamlNodeTextContainer(new HamlLine(-1, "", "   ", HamlRuleEnum.PlainText)),
                new HamlNodeTextContainer(new HamlLine(-1, "", "   ", HamlRuleEnum.PlainText)),
                new HamlNodeTextContainer(new HamlLine(-1, "", "   ", HamlRuleEnum.PlainText)),
                new HamlNodeTag(new HamlLine(-1, "p>", "", HamlRuleEnum.Tag)));

            new HamlDocumentWalker(_mockClassBuilder).Walk(node);

            Assert.That(_mockClassBuilder.Build(""), Is.EqualTo("<p></p>"));
        }

    }
}
