using NHaml.Exceptions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeValueParserTests
    {

        [Test]
        public void CodeSimple()
        {
            var items = AttributeValueParser.Parse(@"#{b}");
            Assert.IsTrue(items[0].IsCode);
            Assert.AreEqual("b", items[0].Value);
        }
        [Test]
        public void EscapedCode()
        {
            var items = AttributeValueParser.Parse(@"\#{b}");
            Assert.IsFalse(items[0].IsCode);
            Assert.AreEqual("#{b}", items[0].Value);
        }
        [Test]
        public void EscapedInsideCode()
        {
            var items = AttributeValueParser.Parse(@"#{\#{b}}");
            Assert.IsTrue(items[0].IsCode);
            Assert.AreEqual("#{b}", items[0].Value);
        }
        [Test]
        public void CodeWithSpaces()
        {
            var items = AttributeValueParser.Parse(@" #{b} ");
            Assert.IsFalse(items[0].IsCode);
            Assert.AreEqual(" ", items[0].Value);
            Assert.IsTrue(items[1].IsCode);
            Assert.AreEqual("b", items[1].Value);
            Assert.IsFalse(items[2].IsCode);
            Assert.AreEqual(" ", items[2].Value);
        }

        [Test]
        public void CodeWithInnerBracket()
        {
            var items = AttributeValueParser.Parse(@"#{b} }");
            Assert.IsTrue(items[0].IsCode);
            Assert.AreEqual("b} ", items[0].Value);
        }

        [Test]
        public void Text()
        {
            var items = AttributeValueParser.Parse(@"abc{$#} sdf#}");
            Assert.IsFalse(items[0].IsCode);
            Assert.AreEqual("abc{$#} sdf#}", items[0].Value);
        }

        [Test]
        public void CodeSurroundedWithTextAndSpaces()
        {
            var items = AttributeValueParser.Parse(@"a #{b} c");
            Assert.IsFalse(items[0].IsCode);
            Assert.AreEqual("a ", items[0].Value);
            Assert.IsTrue(items[1].IsCode);
            Assert.AreEqual("b", items[1].Value);
            Assert.IsFalse(items[2].IsCode);
            Assert.AreEqual(" c", items[2].Value);
        }
        [Test]
        public void CodeSurroundedWithText()
        {
            var items = AttributeValueParser.Parse(@"a#{b}c");
            Assert.IsFalse(items[0].IsCode);
            Assert.AreEqual("a", items[0].Value);
            Assert.IsTrue(items[1].IsCode);
            Assert.AreEqual("b", items[1].Value);
            Assert.IsFalse(items[2].IsCode);
            Assert.AreEqual("c", items[2].Value);
        }
        [Test]
        [ExpectedException(typeof(SyntaxException))]
        public void NoClosingBracket()
        {
            AttributeValueParser.Parse(@"#{");
        }
    }
}