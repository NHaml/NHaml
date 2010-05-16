using System.IO;
using NHaml.Core.Parser;
using NUnit.Framework;
using NHaml.Core.IO;

namespace NHaml.Core.Tests.Parser
{
    [TestFixture]
    public class InputLineReaderReadTests
    {
        private InputLineReader _reader;

        [SetUp]
        public void InputLineReaderSetUp()
        {
            var input = new StringReader("line1\nline2\r\nline3");
            _reader = new InputLineReader(input);
        }

        [Test]
        public void UnReadAllIsNull()
        {
            Assert.Null(_reader.Current);
            Assert.Null(_reader.Next);
            Assert.Null(_reader.Prev);
            Assert.AreEqual(-1, _reader.LineNumber);
            Assert.AreEqual(-1, _reader.LineBeginPosition);
        }

        [Test]
        public void FirstRead()
        {
            Assert.True(_reader.Read());
            Assert.AreEqual(0, _reader.LineNumber);
            Assert.AreEqual(0, _reader.LineBeginPosition);
        }

        [Test]
        public void FirstReadText()
        {
            _reader.Read();

            Assert.Null(_reader.Prev);
            Assert.AreEqual("line1",_reader.Current.Text);
            Assert.AreEqual("line2",_reader.Next.Text);
        }

        [Test]
        public void SecondRead()
        {
            _reader.Read();

            Assert.True(_reader.Read());
            Assert.AreEqual(1, _reader.LineNumber);
            Assert.AreEqual(6, _reader.LineBeginPosition);
        }

        [Test]
        public void SecondReadText()
        {
            _reader.Read();
            _reader.Read();

            Assert.AreEqual("line1", _reader.Prev.Text);
            Assert.AreEqual("line2", _reader.Current.Text);
            Assert.AreEqual("line3", _reader.Next.Text);
        }

        [Test]
        public void ReadLineNumbers()
        {
            _reader.Read();
            _reader.Read();

            Assert.AreEqual(0, _reader.Prev.LineNumber);
            Assert.AreEqual(1, _reader.Current.LineNumber);
            Assert.AreEqual(2, _reader.Next.LineNumber);
        }

        [Test]
        public void ThirdRead()
        {
            _reader.Read();
            _reader.Read();

            Assert.True(_reader.Read());
            Assert.AreEqual(2, _reader.LineNumber);
            Assert.AreEqual(13, _reader.LineBeginPosition);
        }

        [Test]
        public void ThirdReadText()
        {
            _reader.Read();
            _reader.Read();
            _reader.Read();

            Assert.AreEqual("line2", _reader.Prev.Text);
            Assert.AreEqual("line3", _reader.Current.Text);
            Assert.Null(_reader.Next);
        }

        [Test]
        public void FourthRead()
        {
            _reader.Read();
            _reader.Read();
            _reader.Read();

            Assert.False(_reader.Read());
            Assert.AreEqual(3, _reader.LineNumber);
            Assert.AreEqual(18, _reader.LineBeginPosition);
        }

        [Test]
        public void FourthReadText()
        {
            _reader.Read();
            _reader.Read();
            _reader.Read();
            _reader.Read();
            
            Assert.AreEqual("line3", _reader.Prev.Text);
            Assert.Null(_reader.Current);
            Assert.Null(_reader.Next);
        }
    }
}