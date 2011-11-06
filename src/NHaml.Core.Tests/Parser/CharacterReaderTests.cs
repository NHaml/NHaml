using NHaml.Core.Parser;
using NUnit.Framework;
using NHaml.Core.IO;

namespace NHaml.Core.Tests.Parser
{
    [TestFixture]
    public class CharacterReaderTests
    {
        [Test]
        public void Reads4Chars()
        {
            var reader = new CharacterReader("test",0);

            Assert.True(reader.Read());
            Assert.AreEqual('t', reader.CurrentChar);
            Assert.AreEqual('e', reader.NextChar);
            Assert.AreEqual(0, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('e', reader.CurrentChar);
            Assert.AreEqual('s', reader.NextChar);
            Assert.AreEqual(1, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('s', reader.CurrentChar);
            Assert.AreEqual('t', reader.NextChar);
            Assert.AreEqual(2, reader.Index);
            Assert.True(reader.Read());
            Assert.AreEqual('t', reader.CurrentChar);
            Assert.AreEqual(3, reader.Index);
            Assert.False(reader.Read());
        }

        [Test]
        public void ReadToEndReadsTestOneRead()
        {
            var reader = new CharacterReader("test",0);
            reader.Read(); // Current = t

            Assert.AreEqual("test", reader.ReadToEnd());
        }

        [Test]
        public void ReadWhileStopsAtTheFirstCharIfConditionMatch()
        {
            var reader = new CharacterReader("ab",0);
            reader.Read();
            reader.Read();

            reader.ReadWhile(c => char.IsWhiteSpace(c));

            Assert.AreEqual('b', reader.CurrentChar);
        }

        [Test]
        public void ReadWhileStopsAtTheFirstCharIfConditionMatchAtStart()
        {
            var reader = new CharacterReader("ab",0);
            reader.Read();

            reader.ReadWhile(c => char.IsWhiteSpace(c));

            Assert.AreEqual('a', reader.CurrentChar);
        }

        [Test]
        public void ReadsTwoAtStart()
        {
            var reader = new CharacterReader("abcd",0);

            reader.Skip("a");

            Assert.AreEqual('b', reader.CurrentChar);
        }

        [Test]
        public void ReadsTwo()
        {
            var reader = new CharacterReader("abcd",0);
            reader.Read();

            reader.Skip("ab");

            Assert.AreEqual('c', reader.CurrentChar);
        }

        [Test]
        public void ReadToEndReadsTest()
        {
            var reader = new CharacterReader("1test",0);
            reader.Read(); // Current = 1
            reader.Read(); // Current = t

            Assert.AreEqual("test", reader.ReadToEnd());
        }

        [Test]
        public void ReadWhileReadsTestOneRead()
        {
            var reader = new CharacterReader("test1234",0);
            reader.Read(); // Current = t

            var result = reader.ReadWhile(c => char.IsLetter(c));

            Assert.AreEqual("test", result);
        }

        [Test]
        public void ReadWhileReadsTest()
        {
            var reader = new CharacterReader("1test1234",0);
            reader.Read(); // Current = 1
            reader.Read(); // Current = t

            var result = reader.ReadWhile(c => char.IsLetter(c));

            Assert.AreEqual("test", result);
        }

        [Test]
        public void ReadWhileEndsOnTheRightChar()
        {
            var reader = new CharacterReader("test1234",0);
            reader.Read();

            reader.ReadWhile(c => char.IsLetter(c));

            Assert.AreEqual('1', reader.CurrentChar);
        }
    }
}