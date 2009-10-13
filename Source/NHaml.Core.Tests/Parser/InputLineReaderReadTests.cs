using System.IO;
using NHaml.Core.Parser;
using Xunit;

namespace NHaml.Core.Tests.Parser
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
        public void UnReadAllIsNull()
        {
            Assert.Null(_reader.Current);
            Assert.Null(_reader.Next);
            Assert.Null(_reader.Prev);
            Assert.Equal(-1, _reader.LineNumber);
        }

        [Fact]
        public void FirstRead()
        {
            Assert.True(_reader.Read());
            Assert.Equal(0, _reader.LineNumber);
        }

        [Fact]
        public void FirstReadText()
        {
            _reader.Read();

            Assert.Null(_reader.Prev);
            Assert.Equal("line1",_reader.Current.Text);
            Assert.Equal("line2",_reader.Next.Text);
        }

        [Fact]
        public void SecondRead()
        {
            _reader.Read();

            Assert.True(_reader.Read());
            Assert.Equal(1, _reader.LineNumber);
        }

        [Fact]
        public void SecondReadText()
        {
            _reader.Read();
            _reader.Read();

            Assert.Equal("line1", _reader.Prev.Text);
            Assert.Equal("line2", _reader.Current.Text);
            Assert.Equal("line3", _reader.Next.Text);
        }

        [Fact]
        public void ReadLineNumbers()
        {
            _reader.Read();
            _reader.Read();

            Assert.Equal(0, _reader.Prev.LineNumber);
            Assert.Equal(1, _reader.Current.LineNumber);
            Assert.Equal(2, _reader.Next.LineNumber);
        }

        [Fact]
        public void ThiredRead()
        {
            _reader.Read();
            _reader.Read();

            Assert.True(_reader.Read());
            Assert.Equal(2, _reader.LineNumber);
        }

        [Fact]
        public void ThiredReadText()
        {
            _reader.Read();
            _reader.Read();
            _reader.Read();

            Assert.Equal("line2", _reader.Prev.Text);
            Assert.Equal("line3", _reader.Current.Text);
            Assert.Null(_reader.Next);
        }

        [Fact]
        public void ForthRead()
        {
            _reader.Read();
            _reader.Read();
            _reader.Read();

            Assert.False(_reader.Read());
            Assert.Equal(3, _reader.LineNumber);
        }

        [Fact]
        public void ForthReadText()
        {
            _reader.Read();
            _reader.Read();
            _reader.Read();
            _reader.Read();
            
            Assert.Equal("line3", _reader.Prev.Text);
            Assert.Null(_reader.Current);
            Assert.Null(_reader.Next);
        }
    }
}