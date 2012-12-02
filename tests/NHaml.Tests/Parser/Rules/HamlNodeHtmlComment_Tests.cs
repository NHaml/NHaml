using NHaml.IO;
using NHaml.Parser;
using NUnit.Framework;
using NHaml.Parser.Rules;

namespace NHaml.Tests.Parser.Rules
{
    [TestFixture]
    public class HamlNodeHtmlComment_Tests
    {
        [Test]
        public void Constructor_NormalUse_PopulatesCommentTextProperty()
        {
            string comment = "Test comment";

            var node = new HamlNodeHtmlComment(new HamlLine(comment, HamlRuleEnum.HtmlComment, "", 0));
            Assert.That(node.Content, Is.EqualTo(comment));
        }    
    }
}
