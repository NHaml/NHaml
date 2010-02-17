using System.IO;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class TokenTests
    {

        [Test]
        public void MultipleReads()
        {
            
            var queue = new TokenReader(new StringReader("abcd"));
            queue.Read();
            queue.Read();

            var token = queue.Peek();
            Assert.AreEqual('c', token.Character);

        }

        [Test]
        public void ReadAndPeekTokens()
        {
            var queue = new TokenReader(new StringReader("abc\\d"));


            var token = queue.Peek();
            Assert.AreEqual('a', token.Character);

            token = queue.Peek2();
            Assert.AreEqual('b', token.Character);

            token = queue.Read();
            Assert.AreEqual('a', token.Character);

            token = queue.Peek();
            Assert.AreEqual('b', token.Character);

            token = queue.Read();
            Assert.AreEqual('b', token.Character);

            token = queue.Peek();
            Assert.AreEqual('c', token.Character);
            
            token = queue.Read();
            Assert.AreEqual('c', token.Character);

            token = queue.Peek();
            Assert.AreEqual('d', token.Character);
            Assert.IsTrue(token.IsEscaped);
            
            token = queue.Read();
            Assert.AreEqual('d', token.Character);
            Assert.IsTrue(token.IsEscaped);

            
            token = queue.Read();
            Assert.IsTrue(token.IsEnd);
        }

        [Test]
        public void ReadTokens()
        {
            var queue = new TokenReader(new StringReader("abc\\d"));

            var token = queue.Read();
            Assert.AreEqual('a', token.Character);

            token = queue.Read();
            Assert.AreEqual('b', token.Character);

            token = queue.Read();
            Assert.AreEqual('c', token.Character);

            token = queue.Read();
            Assert.AreEqual('d', token.Character);
            Assert.IsTrue(token.IsEscaped);

            
            token = queue.Read();
            Assert.IsTrue(token.IsEnd);
        }
    }
}