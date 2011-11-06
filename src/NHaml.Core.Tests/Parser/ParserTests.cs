using System;
using System.IO;
using NHaml.Core.Parser;
using NUnit.Framework;
using NHaml.Core.Visitors;
using NHaml.Core.Ast;

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
            var document = parser.Parse("@type=string(Stuff=\"value\")\n%p#id.class test\n  #test test");

            Assert.AreEqual("string", document.Metadata["type"][0].Value);
            Assert.AreEqual("value", ((TextChunk)((TextNode)document.Metadata["type"][0].Attributes[0].Value).Chunks[0]).Text);

            new DebugVisitor(wr).Visit(document);

            Assert.AreEqual(@"<p class='class' id='id'>
  test
  <div id='test'>test</div>
</p>", wr.GetStringBuilder().ToString());
        }
    }
}