using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.Walkers.CodeDom;
using NUnit.Framework;
using NHaml.Tests.Mocks;
using Moq;

namespace NHaml.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeHtmlAttributeCollectionWalker_Tests
    {
        [Test]
        [TestCase("()", "")]
        [TestCase("(a='b')", " a=\'b\'")]
        [TestCase("(a=#{test})", " a=\'test\'")]
        [TestCase("(a='b' c='d')", " a=\'b\' c=\'d\'")]
        [TestCase("(class='class1')", "")]
        [TestCase("(id='id1')", "")]
        public void Walk_VaryingAttributeCollections_WritesCorrectAttributes(string hamlLine, string expectedTag)
        {
            var node = new HamlNodeHtmlAttributeCollection(0, hamlLine);

            var builder = new ClassBuilderMock();
            new HamlNodeHtmlAttributeCollectionWalker(builder, new HamlHtmlOptions())
                .Walk(node);

            Assert.That(builder.Build(""), Is.EqualTo(expectedTag));
        }

    }
}
