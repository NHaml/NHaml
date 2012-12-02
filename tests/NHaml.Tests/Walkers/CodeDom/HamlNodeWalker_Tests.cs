using NHaml.Compilers;
using NHaml.Parser;
using NHaml.Walkers.CodeDom;
using NUnit.Framework;
using NHaml.IO;
using NHaml.Parser.Rules;
using NHaml.Tests.Mocks;
using NHaml.Tests.Builders;

namespace NHaml.Tests.Walkers.CodeDom
{
    internal class HamlNodeWalker_Tests
    {
        private ClassBuilderMock _classBuilderMock;
        private DummyWalker _walker;

        private class DummyWalker : HamlNodeWalker
        {
            public DummyWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
                : base(classBuilder, options)
            { }
        }

        [SetUp]
        public void SetUp()
        {
            _classBuilderMock = new ClassBuilderMock();
            _walker = new DummyWalker(_classBuilderMock, new HamlHtmlOptions());
        }

        [Test]
        public void WalkChildren_TextNode_WalksTextNode()
        {
            const string testText = "Hello world";
            var document = HamlDocumentBuilder.Create("",
                new HamlNodeTextContainer(new HamlLine(testText, HamlRuleEnum.PlainText, "", 0)));
            _walker.Walk(document);

            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(testText));
        }

        [Test]
        public void WalkChildren_TagNode_WalksTagNode()
        {
            const string tagName = "div";
            var document = HamlDocumentBuilder.Create("",
                new HamlNodeTag(new HamlLine(tagName, HamlRuleEnum.PlainText, "", 0)));
            _walker.Walk(document);

            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(tagName));
        }

        [Test]
        public void WalkChildren_HtmlCommentNode_WalksHtmlCommentNode()
        {
            const string comment = "test";
            var document = HamlDocumentBuilder.Create("",
                new HamlNodeHtmlComment(new HamlLine(comment, HamlRuleEnum.HtmlComment, "", 0)));

            _walker.Walk(document);

            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(comment));
        }
    }
}
