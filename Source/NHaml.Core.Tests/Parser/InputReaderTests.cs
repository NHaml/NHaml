using System;
using System.IO;
using NHaml.Core.Parser;
using Xunit;

namespace NHaml.Core.Tests.Parser
{
    public class InputReaderTests
    {
        [Fact]
        public void EmptyStringReturnReadFalse()
        {
            var input = new StringReader("");
            var reader = new InputReader(input);
            reader.ReadNextLine();

            Assert.False(reader.Read());
        }

        [Fact]
        public void CanReadOnlyOneChar()
        {
            var input = new StringReader("a");
            var reader = new InputReader(input);

            Assert.True(reader.ReadNextLine());
            Assert.True(reader.Read());
            Assert.Equal('a',reader.CurrentChar);
            Assert.False(reader.Read());
        }

        [Fact]
        public void CanReadTwoLines()
        {
            var input = new StringReader("abc\nxyz");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.Equal('a', reader.CurrentChar);
            Assert.Equal(0, reader.LineNumber);
            Assert.Equal(0, reader.Index);
            Assert.True(reader.Read());
            Assert.Equal('b', reader.CurrentChar);
            Assert.Equal(0, reader.LineNumber);
            Assert.Equal(1, reader.Index);
            Assert.True(reader.Read());
            Assert.Equal('c', reader.CurrentChar);
            Assert.Equal(0, reader.LineNumber);
            Assert.Equal(2, reader.Index);
            Assert.False(reader.Read());
            Assert.False(reader.Read());

            Assert.True(reader.ReadNextLine());
            Assert.True(reader.Read());
            Assert.Equal('x', reader.CurrentChar);
            Assert.Equal(1, reader.LineNumber);
            Assert.Equal(0, reader.Index);
            Assert.True(reader.Read());
            Assert.Equal('y', reader.CurrentChar);
            Assert.Equal(1, reader.LineNumber);
            Assert.Equal(1, reader.Index);
            Assert.True(reader.Read());
            Assert.Equal('z', reader.CurrentChar);
            Assert.Equal(1, reader.LineNumber);
            Assert.Equal(2, reader.Index);
            Assert.False(reader.Read());
        }

        [Fact]
        public void CanReadLineWithIndent()
        {
            var input = new StringReader("  ab");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.Equal('a', reader.CurrentChar);
            Assert.Equal(0, reader.LineNumber);
            Assert.Equal(2, reader.Index);
            Assert.True(reader.Read());
            Assert.Equal('b', reader.CurrentChar);
            Assert.Equal(0, reader.LineNumber);
            Assert.Equal(3, reader.Index);
            Assert.False(reader.Read());
        }

        [Fact]
        public void CanReadToEnd()
        {
            var input = new StringReader("abc\nxyz");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.True(reader.Read()); // skip first char
            Assert.Equal("bc", reader.ReadToEnd());
            Assert.Equal(0,reader.LineNumber);
            Assert.Equal(3,reader.Index);
        }

        [Fact]
        public void CanReadToEnd_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("abc\nxyz");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.Equal("abc", reader.ReadToEnd());
            Assert.Equal(0, reader.LineNumber);
            Assert.Equal(3, reader.Index);
        }

        [Fact]
        public void CanReadNextLineAndReadIfEndOfStream()
        {
            var input = new StringReader("a\n  bc\nd");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.Equal('a', reader.CurrentChar);
            Assert.False(reader.Read());
            Assert.True(reader.ReadNextLineAndReadIfEndOfStream());
            Assert.Equal('b', reader.CurrentChar);
            Assert.True(reader.Read());
            Assert.Equal('c', reader.CurrentChar);
            Assert.False(reader.Read());
            Assert.False(reader.Read());
            Assert.True(reader.ReadNextLineAndReadIfEndOfStream());
            Assert.Equal('d', reader.CurrentChar);
            Assert.False(reader.Read());
            Assert.False(reader.ReadNextLineAndReadIfEndOfStream());
            Assert.False(reader.Read());
        }

        [Fact]
        public void CanReadName()
        {
            var input = new StringReader("so-me_na:me other text");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());

            Assert.Equal("so-me_na:me", reader.ReadName());
            Assert.Equal(11,reader.Index);
        }

        [Fact]
        public void CanReadName_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("so-me_na:me other text");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.Equal("so-me_na:me", reader.ReadName());
            Assert.Equal(11, reader.Index);
        }

        [Fact]
        public void CanReadWithSkiplist()
        {
            var input = new StringReader("abcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.True(reader.Skip("bc"));
            Assert.Equal('c', reader.CurrentChar);
            Assert.Equal(2, reader.Index);
        }

        [Fact]
        public void CanReadWithSkiplist_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("abcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Skip("ab"));
            Assert.Equal('c', reader.CurrentChar);
            Assert.Equal(2, reader.Index);
        }

        [Fact]
        public void CanReadWhile()
        {
            var input = new StringReader("a123bcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.True(reader.Read());
            Assert.Equal("123",reader.ReadWhile(Char.IsNumber));
            Assert.Equal(4,reader.Index);
        }

        [Fact]
        public void CanReadWhile_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("123bcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.Equal("123", reader.ReadWhile(Char.IsNumber));
            Assert.Equal(3, reader.Index);
        }

        [Fact]
        public void CanReadWhiteSpaces()
        {
            var input = new StringReader("a  bc");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.True(reader.Read());
            reader.SkipWhiteSpaces();
            Assert.Equal('b',reader.CurrentChar);
            Assert.Equal(3,reader.Index);
        }

        [Fact]
        public void CanReadWhiteSpaces_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("  abc");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            reader.SkipWhiteSpaces();
            Assert.Equal('a', reader.CurrentChar);
            Assert.Equal(2, reader.Index);
        }
    }
}