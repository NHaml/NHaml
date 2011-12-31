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

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTextWalker_Tests
    {
        private class BogusHamlNode : HamlNode
        {

        }

        [Test]
        public void Walk_NodeIsWrongType_ThrowsException()
        {
            var node = new BogusHamlNode();
            var mockClassBuilder = new Mock<ITemplateClassBuilder>();
            var walker = new HamlNodeTextWalker(mockClassBuilder.Object, new HamlOptions());
            Assert.Throws<InvalidCastException>(() => walker.Walk(node));
        }

        [Test]
        public void Walk_IndentedNode_WritesIndent()
        {
            const string indent = "  ";
            var node = new HamlNodeText(new HamlLine(indent + "Content"));

            var mockClassBuilder = new Mock<ITemplateClassBuilder>();
            var walker = new HamlNodeTextWalker(mockClassBuilder.Object, new HamlOptions());
            walker.Walk(node);

            mockClassBuilder.Verify(x => x.Append(indent));
        }
    }
}
