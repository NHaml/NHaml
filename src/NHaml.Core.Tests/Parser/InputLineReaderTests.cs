using System;
using System.IO;
using NHaml.Core.Parser;
using NUnit.Framework;
using NHaml.Core.IO;

namespace NHaml.Core.Tests.Parser
{
    [TestFixture]
    public class InputLineReaderTests
    {
        [Test]
        public void ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new InputLineReader(null));
        }

        [Test]
        public void EmptyStringShouldNotThrow()
        {
            var input = new StringReader(string.Empty);
            new InputLineReader(input);
        }

        [Test]
        public void EmptyStringShoudWork()
        {
            var input = new StringReader(string.Empty);
            var reader = new InputLineReader(input);

            Assert.False(reader.Read());
        }
    }
}