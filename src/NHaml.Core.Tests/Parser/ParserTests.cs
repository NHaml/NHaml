using System;
using System.IO;
using NHaml.Core.Parser;
using NUnit.Framework;
using NHaml.Core.Visitors;

namespace NHaml.Core.Tests.Parser
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void MetaDataTests()
        {
            StringWriter wr = new StringWriter();
            var parser = new Core.Parser.Parser();
            var document = parser.Parse("@type=string\n%p#id.class test\n  #test test");

            Assert.AreEqual("string", document.Metadata["type"][0]);

            new DebugVisitor(wr).Visit(document);

            Assert.AreEqual(@"<p class='class' id='id'>
  test
  <div id='test'>test</div>
</p>", wr.GetStringBuilder().ToString());
        }
    }
}