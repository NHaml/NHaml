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

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTextWalker_Tests
    {
        private ClassBuilderMock _mockClassBuilder;
        private HamlNodeTextWalker _walker;

        private class BogusHamlNode : HamlNode
        {

        }

        [SetUp]
        public void SetUp()
        {
            _mockClassBuilder = new ClassBuilderMock();
            _walker = new HamlNodeTextWalker(_mockClassBuilder, new HamlOptions());
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
            var node = new HamlNodeText(new HamlLine(indent + "Content"));

            _walker.Walk(node);

            Assert.That(_mockClassBuilder.Build(""), Is.StringStarting(indent));
        }
    }
}
