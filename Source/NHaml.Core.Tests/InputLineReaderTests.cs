using System;
using System.IO;
using NHaml.Core.Parser;
using Xunit;

namespace NHaml.Core.Tests
{
    public class InputLineReaderTests
    {
        [Fact]
        public void ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new InputLineReader(null));
        }

        [Fact]
        public void EmptyStringShouldNotThrow()
        {
            var input = new StringReader(string.Empty);
            new InputLineReader(input);
        }

        [Fact]
        public void EmptyStringShoudWork()
        {
            var input = new StringReader(string.Empty);
            var reader = new InputLineReader(input);

            Assert.False(reader.Read());
        }
    }
}