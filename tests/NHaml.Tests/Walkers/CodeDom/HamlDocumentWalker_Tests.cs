using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;

namespace NHaml4.Tests.Walkers
{
    [TestFixture]
    public class HamlDocumentWalker_Tests
    {
        [Test]
        public void Walk_SimpleFile_AppendsCorrectTag()
        {
            // Arrange
            const string content = "Simple content";
            var classBuilder = new Mock<ITemplateClassBuilder>();
            HamlDocumentWalker walker = new HamlDocumentWalker(classBuilder.Object);
            var document = new HamlDocument();
            document.AddChild(new HamlNodeText(content));

            // Act
            var code = walker.Walk(document, content);

            // Assert
            classBuilder.Verify(x => x.AppendLine(content));
        }

        [Test]
        public void Walk_SingleLineFile_CallsClassBuilderBuild()
        {
            // Arrange
            const string className = "ClassName";
            var classBuilder = new Mock<ITemplateClassBuilder>();
            HamlDocumentWalker walker = new HamlDocumentWalker(classBuilder.Object);
            var document = new HamlTreeParser(new NHaml.IO.HamlFileLexer()).ParseDocument("Simple content");
            
            // Act
            var code = walker.Walk(document, className);

            // Assert
            classBuilder.Verify(x => x.Build(className));
        }
    }
}
