using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTagWalker_Tests
    {
        [Test]
        public void Walk_TagNode_AppendsCorrectTag()
        {
            // Arrange
            const string tagName = "p";
            var tagNode = new HamlNodeTag(tagName);
            var classBuilderMock = new Mock<ITemplateClassBuilder>();

            // Act
            new HamlNodeTagWalker().Walk(tagNode, classBuilderMock.Object);

            // Assert
            classBuilderMock.Verify(x => x.AppendFormat("<{0}{1}></{0}>", tagName, ""));
        }


        [Test]
        public void Walk_TagNodeWithId_AppendsCorrectTag()
        {
            // Arrange
            const string tagLine = "p#id";
            var tagNode = new HamlNodeTag(tagLine);
            var classBuilderMock = new Mock<ITemplateClassBuilder>();

            // Act
            new HamlNodeTagWalker().Walk(tagNode, classBuilderMock.Object);

            // Assert
            const string expectedTag = "p";
            const string expectedAttributes = " id='id'";
            classBuilderMock.Verify(x => x.AppendFormat("<{0}{1}></{0}>", expectedTag, expectedAttributes));
        }
    }
}
