using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Walkers;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser;

namespace NHaml4.Tests.Walkers
{
    [TestFixture]
    public class CodeDomWalker_Tests
    {
        [Test]
        public void Walk_SingleLineFile_AppendsCorrectTag()
        {
            // Arrange
            var classBuilder = new Mock<ITemplateClassBuilder>();
            CodeDomWalker walker = new CodeDomWalker(classBuilder.Object);
            var document = new HamlTreeParser(new NHaml.IO.HamlFileLexer()).ParseDocument("Simple content");
            
            // Act
            var code = walker.Walk(document, "ClassName");

            // Assert
            classBuilder.Verify(x => x.AppendLine("<div>Simple content</div>"));
        }

        [Test]
        public void Walk_SingleLineFile_CallsClassBuilderBuild()
        {
            // Arrange
            const string className = "ClassName";
            var classBuilder = new Mock<ITemplateClassBuilder>();
            CodeDomWalker walker = new CodeDomWalker(classBuilder.Object);
            var document = new HamlTreeParser(new NHaml.IO.HamlFileLexer()).ParseDocument("Simple content");
            
            // Act
            var code = walker.Walk(document, className);

            // Assert
            classBuilder.Verify(x => x.Build(className));
        }
    }
}
