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
        private class HamlNodeDummy : HamlNode {
            public HamlNodeDummy() : base(0, "") { }
        }

        [Test]
        public void AddChild_ValidNode_AddsNodeToChildren()
        {
            var node = new HamlNodeDummy();
            var childNode = new HamlNodeDummy();
            node.AddChild(childNode);
            Assert.AreSame(childNode, node.Children[0]);
        }

        [Test]
        public void Previous_ValidPreviousSibling_ReturnsPreviousSibling()
        {
            var document = new HamlNodeDummy();
            document.AddChild(new HamlNodeDummy());
            document.AddChild(new HamlNodeDummy());

            var result = document.Children[1].Previous;
            Assert.That(result, Is.SameAs(document.Children[0]));
        }

        [Test]
        public void Previous_FirstChild_ReturnsNull()
        {
            var document = new HamlNodeDummy();
            document.AddChild(new HamlNodeDummy());

            var result = document.Children[0].Previous;
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Next_ValidNextSibling_ReturnsNextSibling()
        {
            var document = new HamlNodeDummy();
            document.AddChild(new HamlNodeDummy());
            document.AddChild(new HamlNodeDummy());

            var result = document.Children[0].Next;
            Assert.That(result, Is.SameAs(document.Children[1]));
        }

        [Test]
        public void Parent_ValidChildNode_ReturnsParent()
        {
            var document = new HamlNodeDummy();
            document.AddChild(new HamlNodeDummy());

            var result = document.Children[0].Parent;
            Assert.That(result, Is.SameAs(document));
        }

        [Test]
        public void Parent_RootNode_ReturnsNull()
        {
            var document = new HamlNodeDummy();

            var result = document.Parent;
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Next_LastChild_ReturnsNull()
        {
            var document = new HamlNodeDummy();
            document.AddChild(new HamlNodeDummy());

            var result = document.Children[0].Next;
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Next_ValidChildren_ReturnsNull()
        {
            var document = new HamlNodeDummy();
            document.AddChild(new HamlNodeDummy());
            document.AddChild(new HamlNodeDummy());

            var result = document.Next;
            Assert.That(result, Is.Null);
        }
    }
}
