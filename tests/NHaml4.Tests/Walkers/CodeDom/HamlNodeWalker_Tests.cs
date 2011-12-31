using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;
using NHaml4.IO;
using NHaml4.Parser.Rules;

namespace NHaml4.Tests.Walkers.CodeDom
{
    internal class HamlNodeWalker_Tests
    {
        private Mock<ITemplateClassBuilder> _classBuilderMock;

        private class DummyWalker : HamlNodeWalker
        {
            public DummyWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
                : base(classBuilder, options)
            { }
        }

        [SetUp]
        public void SetUp()
        {
            _classBuilderMock = new Mock<ITemplateClassBuilder>();
        }

        [Test]
        public void WalkChildren_TextNode_WalksTextNode()
        {
            var document = new HamlDocument { new HamlNodeText(new HamlLine("test")) };
            var walker = new DummyWalker(_classBuilderMock.Object, new HamlOptions());
            walker.Walk(document);

            _classBuilderMock.Verify(x => x.Append("test"));
        }

        [Test]
        public void WalkChildren_TagNode_WalksTagNode()
        {
            var document = new HamlDocument { new HamlNodeTag(new HamlLine("test")) };
            var walker = new DummyWalker(_classBuilderMock.Object, new HamlOptions());
            walker.Walk(document);

            _classBuilderMock.Verify(x => x.AppendFormat("<{0}{1}>", "test", ""));
        }

        [Test]
        public void WalkChildren_HtmlCommentNode_WalksHtmlCommentNode()
        {
            var document = new HamlDocument {
                new HamlNodeHtmlComment(new HamlLine("test"))
            };
            var walker = new DummyWalker(_classBuilderMock.Object, new HamlOptions());
            walker.Walk(document);

            _classBuilderMock.Verify(x => x.Append("<!--test"));
        }
    }
}
