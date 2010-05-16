using NHaml.Core.Parser;
using NUnit.Framework;
using NHaml.Core.IO;

namespace NHaml.Core.Tests.Parser
{
    [TestFixture]
    public class InputLineTests
    {
        [Test]
        public void ConstructedRight()
        {
            var line = new InputLine("<line>", 11, 22, 33, 44, true);

            Assert.AreEqual("<line>", line.Text);
            Assert.AreEqual(11, line.StartPosition);
            Assert.AreEqual(22, line.LineNumber);
            Assert.AreEqual(33, line.Indent);
            Assert.AreEqual(44, line.StartIndex);
            Assert.AreEqual(true, line.IsMultiLine);
        }
    }
}