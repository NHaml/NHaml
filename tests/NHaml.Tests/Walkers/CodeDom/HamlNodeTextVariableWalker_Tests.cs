using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.Walkers.CodeDom;
using NUnit.Framework;
using NHaml.Parser;
using NHaml.Compilers;
using Moq;
using NHaml.Parser.Rules;
using NHaml.IO;
using NHaml.Tests.Mocks;

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
        [TestCase("{#Test}", "Test")]
        public void Walk_ValidVariableName_CallsAppendVariableCorrectly(string variableName, string expectedCall)
        {
            var node = new HamlNodeTextVariable(variableName, 0);
            var walker = new HamlNodeTextVariableWalker(_mockClassBuilder.Object, new HamlHtmlOptions());
            walker.Walk(node);
            _mockClassBuilder.Verify(x => x.AppendVariable(expectedCall));
        }
    }
}
