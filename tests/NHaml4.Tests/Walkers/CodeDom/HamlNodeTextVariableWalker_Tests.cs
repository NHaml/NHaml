using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;
using NHaml4.Parser;
using NHaml4.Compilers;
using Moq;
using NHaml4.Parser.Rules;
using NHaml4.IO;
using NHaml4.Tests.Mocks;

namespace NHaml4.Tests.Walkers.CodeDom
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
            var walker = new HamlNodeTextVariableWalker(_mockClassBuilder.Object, new HamlOptions());
            walker.Walk(node);
            _mockClassBuilder.Verify(x => x.AppendVariable(expectedCall));
        }
    }
}
