using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;
using NHaml4.Parser;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTextWalker_Tests
    {
        private class BogusHamlNode : HamlNode
        {

        }

        [Test]
        public void Walk_NodeIsWrongType_ThrowsException()
        {
            var node = new BogusHamlNode();
            Assert.Throws<InvalidCastException>(() => new HamlNodeTextWalker().Walk(node, null));
        }
    }
}
