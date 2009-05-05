using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class ExpressionStringParserTests
    {
        [Test]
        public void Empty()
        {
            var parser = new ExpressionStringParser( "" );

            parser.Parse();

            Assert.AreEqual( 0, parser.Tokens.Count );
        }

        [Test]
        public void OnlyStrings()
        {
            var parser = new ExpressionStringParser( "test string" );

            parser.Parse();

            Assert.AreEqual( 1, parser.Tokens.Count );

            Assert.AreEqual("test string", parser.Tokens[0].Value);
            Assert.IsFalse(parser.Tokens[0].IsExpression);
        }

        [Test]
        public void OneExpression()
        {
            var parser = new ExpressionStringParser( "#{test}" );

            parser.Parse();

            Assert.AreEqual( 1, parser.Tokens.Count );

            Assert.AreEqual( "test", parser.Tokens[0].Value );
            Assert.IsTrue( parser.Tokens[0].IsExpression );
        }

        [Test]
        public void Mixed()
        {
            var parser = new ExpressionStringParser( "string #{test} abc {} #{cba}#{bca}#{ y" );

            parser.Parse();

            Assert.AreEqual( 6, parser.Tokens.Count );

            Assert.AreEqual( "string ", parser.Tokens[0].Value );
            Assert.IsFalse( parser.Tokens[0].IsExpression );
            Assert.AreEqual( "test", parser.Tokens[1].Value );
            Assert.IsTrue( parser.Tokens[1].IsExpression );
            Assert.AreEqual( " abc {} ", parser.Tokens[2].Value );
            Assert.IsFalse( parser.Tokens[2].IsExpression );
            Assert.AreEqual( "cba", parser.Tokens[3].Value );
            Assert.IsTrue( parser.Tokens[3].IsExpression );
            Assert.AreEqual( "bca", parser.Tokens[4].Value );
            Assert.IsTrue( parser.Tokens[4].IsExpression );
            Assert.AreEqual( "#{ y", parser.Tokens[5].Value );
            Assert.IsFalse( parser.Tokens[5].IsExpression );
        }

        [Test]
        public void EscapedExpression()
        {
            var parser = new ExpressionStringParser( @"#{abc\}\{\}}" );

            parser.Parse();

            Assert.AreEqual( 1, parser.Tokens.Count );

            Assert.AreEqual( "abc}{}", parser.Tokens[0].Value );
            Assert.IsTrue( parser.Tokens[0].IsExpression );
        }

    }
}