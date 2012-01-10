using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Parser.Rules;
using NHaml4.Walkers.CodeDom;
using NHaml4.Tests.Mocks;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeHtmlAttributeCollectionWalker_Tests
    {
        [Test]
        [TestCase("()", "")]
        [TestCase("(a='b')", " a='b'")]
        [TestCase("(a='b' c='d')", " a='b' c='d'")]
        [TestCase("(class='class1')", "")]
        [TestCase("(id='id1')", "")]
        public void Walk_EmptyAttributeCollection_WritesCorrectAttributes(string hamlLine, string expectedTag)
        {
            var node = new HamlNodeHtmlAttributeCollection(hamlLine);

            var builder = new ClassBuilderMock();
            new HamlNodeHtmlAttributeCollectionWalker(builder, new HamlOptions())
                .Walk(node);

            Assert.That(builder.Build(""), Is.EqualTo(expectedTag));
        }
    }
}
