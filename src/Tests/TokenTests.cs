using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class TokenTests
    {


        [Test]
        public void ReadAllTokens()
        {
            var queue = Token.ReadAllTokens("abc\\d");
            Assert.AreEqual(5, queue.Count);
            Assert.AreEqual('a', queue.First.Value.Character);
            queue.RemoveFirst();
            Assert.AreEqual('b', queue.First.Value.Character);
            queue.RemoveFirst();
            Assert.AreEqual('c', queue.First.Value.Character);
            queue.RemoveFirst();
            var dequeue = queue.First.Value;
            queue.RemoveFirst();
            Assert.AreEqual('d', dequeue.Character);
            Assert.IsTrue(dequeue.IsEscaped);
            Assert.IsTrue(queue.First.Value.IsEnd);
        }
    }
}