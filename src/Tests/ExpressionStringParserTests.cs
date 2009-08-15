using NHaml.Exceptions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class ExpressionStringParserTests
    {
        [Test]
        public void Empty()
        {
            var parser = new ExpressionStringParser("");

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("", parser.ExpressionStringTokens[0].Value);
            Assert.IsFalse(parser.ExpressionStringTokens[0].IsExpression);
        }

        [Test]
        public void OnlyStrings()
        {
            var parser = new ExpressionStringParser("test string");

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("test string", parser.ExpressionStringTokens[0].Value);
            Assert.IsFalse(parser.ExpressionStringTokens[0].IsExpression);
        }

        [Test]
        public void Temp()
        {
            var parser = new ExpressionStringParser("#{string.Join(\"-\", new string[]{\"1\",\"2\",\"3\"\\})}");

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);
        }
        [Test]
        public void OneExpression()
        {
            var parser = new ExpressionStringParser("#{test}");

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("test", parser.ExpressionStringTokens[0].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[0].IsExpression);
        }

        [Test]
        public void Mixed()
        {
            var parser = new ExpressionStringParser("string #{test} abc {} #{cba}#{bca}#{ y}");

            parser.Parse();

            Assert.AreEqual(6, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("string ", parser.ExpressionStringTokens[0].Value);
            Assert.IsFalse(parser.ExpressionStringTokens[0].IsExpression);
            Assert.AreEqual("test", parser.ExpressionStringTokens[1].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[1].IsExpression);
            Assert.AreEqual(" abc {} ", parser.ExpressionStringTokens[2].Value);
            Assert.IsFalse(parser.ExpressionStringTokens[2].IsExpression);
            Assert.AreEqual("cba", parser.ExpressionStringTokens[3].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[3].IsExpression);
            Assert.AreEqual("bca", parser.ExpressionStringTokens[4].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[4].IsExpression);
            Assert.AreEqual(" y", parser.ExpressionStringTokens[5].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[5].IsExpression);
        }

        [Test]
        public void SimpleMixed()
        {
            var parser = new ExpressionStringParser(" a #{b } ");

            parser.Parse();

            Assert.AreEqual(3, parser.ExpressionStringTokens.Count);

            var token = parser.ExpressionStringTokens[0];
            Assert.AreEqual(" a ", token.Value);
            Assert.IsFalse(token.IsExpression);

            token = parser.ExpressionStringTokens[1];
            Assert.AreEqual("b ", token.Value);
            Assert.IsTrue(token.IsExpression);

            token = parser.ExpressionStringTokens[2];
            Assert.AreEqual(" ", token.Value);
            Assert.IsFalse(token.IsExpression);
        }


        [Test]
        [ExpectedException(typeof(SyntaxException))]
        public void MissingTrailingCurly()
        {
            var parser = new ExpressionStringParser("#{b ");
            parser.Parse();
        }

        [Test]
        public void EscapedExpression()
        {
            var parser = new ExpressionStringParser(@"#{abc\}\{\}}");

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("abc}{}", parser.ExpressionStringTokens[0].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[0].IsExpression);
        }

    }
}