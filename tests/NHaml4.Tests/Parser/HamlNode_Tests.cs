using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml4.IO;

namespace NHaml4.Tests.Parser
{
    [TestFixture]
    public class HamlNode_Tests
    {
        private class HamlNodeDummy : HamlNode { }

        [Test]
        public void AddChild_ValidNode_AddsNodeToChildren()
        {
            var node = new HamlNodeDummy();
            var childNode = new HamlNodeDummy();
            node.AddChild(childNode);
            Assert.AreSame(childNode, node.Children[0]);
        }
    }
}
