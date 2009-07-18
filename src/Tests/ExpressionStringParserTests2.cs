using NHaml.Exceptions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class ExpressionStringParserTests2
    {
        [Test]
        public void Empty()
        {
            var parser = new ExpressionStringParser2( "" );

            parser.Parse();

            Assert.AreEqual(0, parser.ExpressionStringTokens.Count);

        }

        [Test]
        public void OnlyStrings()
        {
            var parser = new ExpressionStringParser2( "test string" );

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("test string", parser.ExpressionStringTokens[0].Value);
            Assert.IsFalse(parser.ExpressionStringTokens[0].IsExpression);
        }

        [Test]
        public void OneExpression()
        {
            var parser = new ExpressionStringParser2( "#{test}" );

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("test", parser.ExpressionStringTokens[0].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[0].IsExpression);
        }

        [Test]
        public void Mixed()
        {
            var parser = new ExpressionStringParser2( "string #{test} abc {} #{cba}#{bca}#{ y}" );

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
            var parser = new ExpressionStringParser2( " a #{b } " );

            parser.Parse();

            Assert.AreEqual(3, parser.ExpressionStringTokens.Count);

            Assert.AreEqual(" a ", parser.ExpressionStringTokens[0].Value);
            Assert.IsFalse(parser.ExpressionStringTokens[0].IsExpression);
            Assert.AreEqual("b ", parser.ExpressionStringTokens[1].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[1].IsExpression);
            Assert.AreEqual(" ", parser.ExpressionStringTokens[2].Value);
            Assert.IsFalse(parser.ExpressionStringTokens[2].IsExpression);
        }
        [Test]
        [ExpectedException(typeof(SyntaxException))]
        public void MissingTrailingCurly()
        {
            var parser = new ExpressionStringParser2( "#{b " );
            parser.Parse();
        }

        [Test]
        public void EscapedExpression()
        {
            var parser = new ExpressionStringParser2( @"#{abc\}\{\}}" );

            parser.Parse();

            Assert.AreEqual(1, parser.ExpressionStringTokens.Count);

            Assert.AreEqual("abc}{}", parser.ExpressionStringTokens[0].Value);
            Assert.IsTrue(parser.ExpressionStringTokens[0].IsExpression);
        }

    }
}