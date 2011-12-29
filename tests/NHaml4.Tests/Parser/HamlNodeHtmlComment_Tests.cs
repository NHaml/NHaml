using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;
using NHaml4.Parser;
using NUnit.Framework;

namespace NHaml4.Tests.Parser
{
    [TestFixture]
    public class HamlNodeHtmlComment_Tests
    {
        [Test]
        public void Constructor_NormalUse_PopulatesCommentTextProperty()
        {
            string comment = "Test comment";

            var node = new HamlNodeHtmlComment(new HamlLine("/" + comment));
            Assert.That(node.CommentText, Is.EqualTo(comment));
        }    
    }
}
