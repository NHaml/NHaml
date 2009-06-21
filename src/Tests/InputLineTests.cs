using NUnit.Framework;

namespace NHaml.Tests
{
  
    [TestFixture]
    public class InputLineTests
    {
        [Test]
        public void DocType()
        {
            var inputLine = new InputLine( "!!!", 1 );

            Assert.AreEqual( 0, inputLine.IndentCount );
            Assert.AreEqual( string.Empty, inputLine.Indent );
            Assert.AreEqual( "!!!", inputLine.Text );
        }

        [Test]
        public void Trimmed()
        {
            var inputLine = new InputLine( "  %head ", 1 );

            Assert.AreEqual( "  %head ", inputLine.Text );
        }

        [Test]
        public void Indent()
        {
            var inputLine = new InputLine( "%head", 1 );

            Assert.AreEqual( 0, inputLine.IndentCount );
            Assert.AreEqual( string.Empty, inputLine.Indent );
            Assert.AreEqual( "%head", inputLine.Text );

            inputLine = new InputLine( "  %head", 1 );

            Assert.AreEqual( 1, inputLine.IndentCount );
            Assert.AreEqual( "  ", inputLine.Indent );
            Assert.AreEqual("  %head", inputLine.Text);

            inputLine = new InputLine( "    %body", 1 );

            Assert.AreEqual( 2, inputLine.IndentCount );
            Assert.AreEqual( "    ", inputLine.Indent );
            Assert.AreEqual("    %body", inputLine.Text);
        }
    }
}