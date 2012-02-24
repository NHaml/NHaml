using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Walkers.CodeDom;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser.Rules;
using NHaml4.IO;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeEvalWalker_Tests
    {
        [Test]
        public void Walk_ValidNode_CallsAppendCodeSnippetMethod()
        {
            var classBuilderMock = new Mock<ITemplateClassBuilder>();
            var walker = new HamlNodeEvalWalker(classBuilderMock.Object, new HamlHtmlOptions());
            string codeSnippet = "1+1";
            var node = new HamlNodeEval(new HamlLine(codeSnippet, -1));
            walker.Walk(node);

            classBuilderMock.Verify(x => x.AppendCode(codeSnippet));
        }
    }
}
