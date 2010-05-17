using System;
using System.IO;
using NHaml.Core.Parser;
using NUnit.Framework;
using NHaml.Core.IO;

namespace NHaml.Core.Tests.Parser
{
    [TestFixture]
    public class InputReaderTests
    {
        [Test]
        public void EmptyStringReturnReadFalse()
        {
            var input = new StringReader("");
            var reader = new InputReader(input);
            reader.ReadNextLine();

            Assert.False(reader.Read());
        }

        [Test]
        public void CanReadOnlyOneChar()
        {
            var input = new StringReader("a");
            var reader = new InputReader(input);

            Assert.True(reader.ReadNextLine());
            Assert.True(reader.Read());
            Assert.AreEqual('a',reader.CurrentChar);
            Assert.False(reader.Read());
        }

        [Test]
        public void CanReadTwoLines()
        {
            var input = new StringReader("abc\nxyz");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.AreEqual('a', reader.CurrentChar);
            Assert.AreEqual(0, reader.LineNumber);
            Assert.AreEqual(0, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('b', reader.CurrentChar);
            Assert.AreEqual(0, reader.LineNumber);
            Assert.AreEqual(1, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('c', reader.CurrentChar);
            Assert.AreEqual(0, reader.LineNumber);
            Assert.AreEqual(2, reader.Index);
            Assert.False(reader.Read());
            Assert.False(reader.Read());

            Assert.True(reader.ReadNextLine());
            Assert.True(reader.Read());
            Assert.AreEqual('x', reader.CurrentChar);
            Assert.AreEqual(1, reader.LineNumber);
            Assert.AreEqual(0, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('y', reader.CurrentChar);
            Assert.AreEqual(1, reader.LineNumber);
            Assert.AreEqual(1, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('z', reader.CurrentChar);
            Assert.AreEqual(1, reader.LineNumber);
            Assert.AreEqual(2, reader.Index);
            Assert.False(reader.Read());
        }

        [Test]
        public void CanReadLineWithIndent()
        {
            var input = new StringReader("  ab");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.AreEqual('a', reader.CurrentChar);
            Assert.AreEqual(0, reader.LineNumber);
            Assert.AreEqual(2, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('b', reader.CurrentChar);
            Assert.AreEqual(0, reader.LineNumber);
            Assert.AreEqual(3, reader.Index);
            Assert.False(reader.Read());
        }

        [Test]
        public void CanReadToEnd()
        {
            var input = new StringReader("abc\nxyz");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.True(reader.Read()); // skip first char
            Assert.AreEqual("bc", reader.ReadToEnd());
            Assert.AreEqual(0,reader.LineNumber);
            Assert.AreEqual(3,reader.Index);
        }

        [Test]
        public void CanReadToEnd_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("abc\nxyz");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.AreEqual("abc", reader.ReadToEnd());
            Assert.AreEqual(0, reader.LineNumber);
            Assert.AreEqual(3, reader.Index);
        }

        [Test]
        public void CanReadNextLineAndReadIfEndOfStream()
        {
            var input = new StringReader("a\n  bc\nd");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.AreEqual('a', reader.CurrentChar);
            Assert.False(reader.Read());
            Assert.True(reader.ReadNextLineAndReadIfEndOfStream());
            Assert.AreEqual('b', reader.CurrentChar);
            Assert.True(reader.Read());
            Assert.AreEqual('c', reader.CurrentChar);
            Assert.False(reader.Read());
            Assert.False(reader.Read());
            Assert.True(reader.ReadNextLineAndReadIfEndOfStream());
            Assert.AreEqual('d', reader.CurrentChar);
            Assert.False(reader.Read());
            Assert.False(reader.ReadNextLineAndReadIfEndOfStream());
            Assert.False(reader.Read());
        }

        [Test]
        public void CanReadName()
        {
            var input = new StringReader("so-me_na:me other text");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());

            Assert.AreEqual("so-me_na:me", reader.ReadName());
            Assert.AreEqual(11,reader.Index);
        }

        [Test]
        public void CanReadName_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("so-me_na:me other text");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.AreEqual("so-me_na:me", reader.ReadName());
            Assert.AreEqual(11, reader.Index);
        }

        [Test]
        public void CanReadWithSkiplist()
        {
            var input = new StringReader("abcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());
            Assert.True(reader.Read());

            Assert.True(reader.Read());
            Assert.True(reader.Skip("bc"));
            Assert.AreEqual('d', reader.CurrentChar);
            Assert.AreEqual(3, reader.Index);
        }

        [Test]
        public void CanReadWithSkiplist_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("abcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Skip("ab"));
            Assert.AreEqual('c', reader.CurrentChar);
            Assert.AreEqual(2, reader.Index);
        }

        [Test]
        public void CanReadWhile()
        {
            var input = new StringReader("a123bcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.True(reader.Read());
            Assert.AreEqual("123",reader.ReadWhile(Char.IsNumber));
            Assert.AreEqual(4,reader.Index);
        }

        [Test]
        public void CanReadWhile_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("123bcdef");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.AreEqual("123", reader.ReadWhile(Char.IsNumber));
            Assert.AreEqual(3, reader.Index);
        }

        [Test]
        public void CanReadWhiteSpaces()
        {
            var input = new StringReader("a  bc");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            Assert.True(reader.Read());
            Assert.True(reader.Read());
            reader.SkipWhiteSpaces();
            Assert.AreEqual('b',reader.CurrentChar);
            Assert.AreEqual(3,reader.Index);
        }

        [Test]
        public void CanReadWhiteSpaces_AutomaticlyMakesAnInitialReadOnStart()
        {
            var input = new StringReader("  abc");
            var reader = new InputReader(input);
            Assert.True(reader.ReadNextLine());

            reader.SkipWhiteSpaces();
            Assert.AreEqual('a', reader.CurrentChar);
            Assert.AreEqual(2, reader.Index);
        }
    }
}