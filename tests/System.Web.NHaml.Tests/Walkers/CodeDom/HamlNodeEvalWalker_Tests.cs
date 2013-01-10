using System.Web.NHaml.Compilers;
using System.Web.NHaml.IO;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.Walkers.CodeDom;
using System.Web.NHaml.Walkers.Exceptions;
using NUnit.Framework;
using Moq;

namespace NHaml.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeEvalWalker_Tests
    {
        private Mock<ITemplateClassBuilder> _classBuilderMock;
        private HamlNodeEvalWalker _walker;

        [SetUp]
        public void Setup()
        {
            _classBuilderMock = new Mock<ITemplateClassBuilder>();
            _walker = new HamlNodeEvalWalker(_classBuilderMock.Object, new HamlHtmlOptions());
        }

        [Test]
        public void Walk_ValidNode_CallsAppendCodeToStringMethod()
        {
            string codeSnippet = "1+1";
            var node = new HamlNodeEval(new HamlLine(codeSnippet, HamlRuleEnum.PlainText, "", -1));
            
            _walker.Walk(node);

            _classBuilderMock.Verify(x => x.AppendCodeToString(codeSnippet));
        }

        [Test]
        public void Walk_ChildNode_ThrowsInvalidChildNodeException()
        {
            string codeSnippet = "1+1";
            var node = new HamlNodeEval(new HamlLine(codeSnippet, HamlRuleEnum.PlainText, "", -1));
            node.AddChild(new HamlNodeTextContainer(0, ""));

            Assert.Throws<HamlInvalidChildNodeException>(() => _walker.Walk(node));
        }
    }
}
