using System.IO;
using NHaml.Core.Parser;
using Xunit;

namespace NHaml.Core.Tests
{
    public class InputLineReaderReadTests
    {
        private readonly InputLineReader _reader;

        public InputLineReaderReadTests()
        {
            var input = new StringReader("line1\nline2\nline3");
            _reader = new InputLineReader(input);
        }

        [Fact]
        public void UnstartedAllIsNull()
        {
            Assert.Null(_reader.Current);
            Assert.Null(_reader.Next);
            Assert.Null(_reader.Prev);
            Assert.Equal(-1, _reader.LineNumber);
        }

        [Fact]
        public void FirstReadLines()
        {
            Assert.True(_reader.Read());
            Assert.Equal(0, _reader.LineNumber);

            Assert.Null(_reader.Prev);
            Assert.Equal("line1",_reader.Current.ToString());
            Assert.Equal("line2",_reader.Next.ToString());
        }

        [Fact]
        public void FirstReadLineNumbers()
        {
            Assert.True(_reader.Read());

            Assert.Null(_reader.Prev);
            Assert.Equal("line1", _reader.Current.LineNumber);
            Assert.Equal("line2", _reader.Next.LineNumber);

        }

        [Fact]
        public void SecondRead()
        {
            Assert.True(_reader.Read());
            Assert.True(_reader.Read());
            Assert.Equal(1, _reader.LineNumber);

            Assert.Equal("line1", _reader.Prev.ToString());
            Assert.Equal("line2", _reader.Current.ToString());
            Assert.Equal("line3", _reader.Next.ToString());
        }

        [Fact]
        public void ThiredRead()
        {
            Assert.True(_reader.Read());
            Assert.True(_reader.Read());
            Assert.True(_reader.Read());
            Assert.Equal(2, _reader.LineNumber);

            Assert.Equal("line2", _reader.Prev.ToString());
            Assert.Equal("line3", _reader.Current.ToString());
            Assert.Null(_reader.Next);
        }

        [Fact]
        public void ForuthRead()
        {
            Assert.True(_reader.Read());
            Assert.True(_reader.Read());
            Assert.True(_reader.Read());
            Assert.False(_reader.Read());
            Assert.Equal(3, _reader.LineNumber);

            Assert.Equal("line3", _reader.Prev.ToString());
            Assert.Null(_reader.Current);
            Assert.Null(_reader.Next);
        }
    }
}