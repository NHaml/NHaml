using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeParserTests
    {
        [Test]
        public void CodeWithNumbers()
        {
            var attributeParser = new AttributeParser("a2=#{b2}");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a2", value.Key);
            Assert.AreEqual("b2", value.Value);
            Assert.IsTrue(value.IsCode);
        }
        [Test]
        public void CodeWithComma()
        {
            var attributeParser = new AttributeParser("a=#{b,c}");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a", value.Key);
            Assert.AreEqual("b,c", value.Value);
            Assert.IsTrue(value.IsCode);
        }
        [Test]
        public void CodeWithSingleQuote()
        {
            var attributeParser = new AttributeParser("a=#{b'c}");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a", value.Key);
            Assert.AreEqual("b'c", value.Value);
            Assert.IsTrue(value.IsCode);
        }
        [Test]
        public void CodeWithDoubleQuote()
        {
            var attributeParser = new AttributeParser("a=#{b\"c}");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a", value.Key);
            Assert.AreEqual("b\"c", value.Value);
            Assert.IsTrue(value.IsCode);
        }
        [Test]
        public void CodeWithEquals()
        {
            var attributeParser = new AttributeParser("a=#{b=c}");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a", value.Key);
            Assert.AreEqual("b=c", value.Value);
            Assert.IsTrue(value.IsCode);
        }
        [Test]
        public void CodeWithColon()
        {
            var attributeParser = new AttributeParser("a:b=#{c}");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a:b", value.Key);
            Assert.AreEqual("c", value.Value);
            Assert.IsTrue(value.IsCode);
        }
        [Test]
        public void CodeSimple()
        {
            var attributeParser = new AttributeParser("foo=#{bar}");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("foo", value.Key);
            Assert.AreEqual("bar", value.Value);
            Assert.IsTrue(value.IsCode);
        }
        [Test]
        public void MultipleCode()
        {
            var attributeParser = new AttributeParser("a=#{b},c=#{d}");
            attributeParser.Parse();

            var value1 = attributeParser.Values[0];
            Assert.AreEqual("a", value1.Key);
            Assert.AreEqual("b", value1.Value);
            Assert.IsTrue(value1.IsCode);
            var value2 = attributeParser.Values[1];
            Assert.AreEqual("c", value2.Key);
            Assert.AreEqual("d", value2.Value);
            Assert.IsTrue(value2.IsCode);
        }
        [Test]
        public void MultipleCodeWithLotsOfSpaces()
        {
            var attributeParser = new AttributeParser(" a = #{b} , c = #{d} ");
            attributeParser.Parse();

            var value1 = attributeParser.Values[0];
            Assert.AreEqual("a", value1.Key);
            Assert.AreEqual("b", value1.Value);
            Assert.IsTrue(value1.IsCode);
            var value2 = attributeParser.Values[1];
            Assert.AreEqual("c", value2.Key);
            Assert.AreEqual("d", value2.Value);
            Assert.IsTrue(value2.IsCode);
        }

        [Test]
        public void SimpleText()
        {
            var attributeParser = new AttributeParser("foo='bar'");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("foo", value.Key);
            Assert.AreEqual("bar", value.Value);
            Assert.IsFalse(value.IsCode);
        }
        [Test]
        public void SimpleText2()
        {
            var attributeParser = new AttributeParser("foo=\"bar\"");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("foo", value.Key);
            Assert.AreEqual("bar", value.Value);
            Assert.IsFalse(value.IsCode);
        }
        [Test]
        public void TextWithNumbers()
        {
            var attributeParser = new AttributeParser("a2='b2'");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a2", value.Key);
            Assert.AreEqual("b2", value.Value);
            Assert.IsFalse(value.IsCode);
        }
        [Test]
        public void TextWithSingleQuote()
        {
            var attributeParser = new AttributeParser("a2='b'2'");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a2", value.Key);
            Assert.AreEqual("b'2", value.Value);
            Assert.IsFalse(value.IsCode);
        }
        [Test]
        public void TextWithDoubleQuote()
        {
            var attributeParser = new AttributeParser("a2='b\"2'");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a2", value.Key);
            Assert.AreEqual("b\"2", value.Value);
            Assert.IsFalse(value.IsCode);
        }
        [Test]
        public void TextWithColon()
        {
            var attributeParser = new AttributeParser("a:b='c'");
            attributeParser.Parse();

            var value = attributeParser.Values[0];
            Assert.AreEqual("a:b", value.Key);
            Assert.AreEqual("c", value.Value);
            Assert.IsFalse(value.IsCode);
        }
        [Test]
        public void MultipleText()
        {
            var attributeParser = new AttributeParser("a='b',c='d'");
            attributeParser.Parse();

            var value1 = attributeParser.Values[0];
            Assert.AreEqual("a", value1.Key);
            Assert.AreEqual("b", value1.Value);
            Assert.IsFalse(value1.IsCode);
            var value2 = attributeParser.Values[1];
            Assert.AreEqual("c", value2.Key);
            Assert.AreEqual("d", value2.Value);
            Assert.IsFalse(value2.IsCode);
        }
        [Test]
        public void MultipleTextWithLotsOfSpaces()
        {
            var attributeParser = new AttributeParser(" a = 'b' , c = 'd' ");
            attributeParser.Parse();

            var value1 = attributeParser.Values[0];
            Assert.AreEqual("a", value1.Key);
            Assert.AreEqual("b", value1.Value);
            Assert.IsFalse(value1.IsCode);
            var value2 = attributeParser.Values[1];
            Assert.AreEqual("c", value2.Key);
            Assert.AreEqual("d", value2.Value);
            Assert.IsFalse(value2.IsCode);
        }
    }
}