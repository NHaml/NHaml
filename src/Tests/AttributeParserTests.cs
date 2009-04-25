using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeParserTests
    {
        [Test]
        public void SingleSingleQuotes()
        {
            var attributeParser = new AttributeParser("a='b'");
            attributeParser.Parse();
            Assert.IsTrue(attributeParser.Values.ContainsKey("a"));
            Assert.AreEqual("b", attributeParser.Values["a"]);
        }
        [Test]
        public void SingleDoubleQuotes()
        {
            var attributeParser = new AttributeParser("a=\"b\"");
            attributeParser.Parse();
            Assert.IsTrue(attributeParser.Values.ContainsKey("a"));
            Assert.AreEqual("b", attributeParser.Values["a"]);
        }
        [Test]
        public void MultipleSingleQuotes()
        {
            var attributeParser = new AttributeParser("a='b' c='d' e='f'");
            attributeParser.Parse();
            Assert.IsTrue(attributeParser.Values.ContainsKey("a"));
            Assert.AreEqual("b", attributeParser.Values["a"]);
            Assert.IsTrue(attributeParser.Values.ContainsKey("c"));
            Assert.AreEqual("d", attributeParser.Values["c"]);
            Assert.IsTrue(attributeParser.Values.ContainsKey("e"));
            Assert.AreEqual("f", attributeParser.Values["e"]);
        }
        [Test]
        public void MultipleDoubleQuotes()
        {
            var attributeParser = new AttributeParser("a=\"b\" c=\"d\" e=\"f\" ");
            attributeParser.Parse();
            Assert.IsTrue(attributeParser.Values.ContainsKey("a"));
            Assert.AreEqual("b", attributeParser.Values["a"]);
            Assert.IsTrue(attributeParser.Values.ContainsKey("c"));
            Assert.AreEqual("d", attributeParser.Values["c"]);
            Assert.IsTrue(attributeParser.Values.ContainsKey("e"));
            Assert.AreEqual("f", attributeParser.Values["e"]);
        }
    }
}