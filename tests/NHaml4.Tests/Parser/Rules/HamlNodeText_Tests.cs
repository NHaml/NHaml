using NUnit.Framework;
using NHaml4.Parser.Rules;
using NHaml4.IO;
namespace NHaml4.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeText_Tests
    {
        [Test]
        [TestCase("Test", false)]
        [TestCase("   ", true)]
        [TestCase("\n", true)]
        [TestCase("\t", true)]
        public void IsWhitespace_ReturnsCorrectResult(string whiteSpace, bool expectedResult)
        {
            var node = new HamlNodeText(new HamlLine(whiteSpace, 0));
            Assert.That(node.IsWhitespace(), Is.EqualTo(expectedResult));
        }
    }
}
