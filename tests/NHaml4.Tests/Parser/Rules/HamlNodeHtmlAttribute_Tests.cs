using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Parser.Rules;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeHtmlAttribute_Tests
    {
        [Test]
        [TestCase("a='b'", "a", "'b'")]
        [TestCase("a", "a", "")]
        public void Constructor_NormalUse_GeneratesCorrectNameValuePair(string input, string name, string value)
        {
            var node = new HamlNodeHtmlAttribute(input);
            Assert.That(node.Name, Is.EqualTo(name));
            Assert.That(node.Value, Is.EqualTo(value));
        }

        public void Constructor_MalformedAttribute_ThrowsException()
        {
            Assert.Throws<HamlMalformedTagException>(() => new HamlNodeHtmlAttribute("=b"));

        }
    }
}
