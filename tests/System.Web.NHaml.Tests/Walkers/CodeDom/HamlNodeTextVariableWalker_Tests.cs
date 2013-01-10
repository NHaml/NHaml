using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.Walkers.CodeDom;
using NUnit.Framework;
using Moq;

namespace NHaml.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTextVariableWalker_Tests
    {
        private Mock<ITemplateClassBuilder> _mockClassBuilder;

        [SetUp]
        public void SetUp()
        {
            _mockClassBuilder = new Mock<ITemplateClassBuilder>();
        }

        [Test]
        [TestCase("#{Test}", "Test")]
        public void Walk_SimpleVariableName_CallsAppendVariableCorrectly(string variableName, string expectedCall)
        {
            var node = new HamlNodeTextVariable(0, variableName);
            var walker = new HamlNodeTextVariableWalker(_mockClassBuilder.Object, new HamlHtmlOptions());
            walker.Walk(node);
            _mockClassBuilder.Verify(x => x.AppendVariable(expectedCall));
        }

        [Test]
        [TestCase("#{Model.Blah}", "Model.Blah")]
        [TestCase("#{Model[0]}", "Model[0]")]
        [TestCase("#{Model[0].Blah}", "Model[0].Blah")]
        [TestCase("#{new Object()}", "new Object()")]
        public void Walk_ComplexPropertyName_CallsCodeSnippetToStringCorrectly(string variableName, string expectedCodeSnippet)
        {
            var node = new HamlNodeTextVariable(0, variableName);
            var walker = new HamlNodeTextVariableWalker(_mockClassBuilder.Object, new HamlHtmlOptions());
            walker.Walk(node);
            _mockClassBuilder.Verify(x => x.AppendCodeToString(expectedCodeSnippet));
        }
    }
}
