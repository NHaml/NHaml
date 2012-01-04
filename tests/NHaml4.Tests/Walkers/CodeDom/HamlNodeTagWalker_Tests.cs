using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NHaml4.Compilers;
using NHaml4.Parser;
using NHaml4.Walkers.CodeDom;
using NUnit.Framework;
using NHaml4.IO;
using NHaml4.Parser.Rules;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTagWalker_Tests
    {
        Mock<ITemplateClassBuilder> _classBuilderMock;
        private HamlNodeTagWalker _tagWalker;
        private HamlOptions _hamlOptions;

        [SetUp]
        public void SetUp()
        {
            _classBuilderMock = new Mock<ITemplateClassBuilder>();
            _hamlOptions = new HamlOptions();
            _tagWalker = new HamlNodeTagWalker(_classBuilderMock.Object, _hamlOptions);
        }

        [Test]
        [TestCase("p", "p", "", "")]
        [TestCase("p#id", "p", "id", "")]
        [TestCase("p.class", "p", "", "class")]
        [TestCase("ns:id", "ns:id", "", "")]
        public void Walk_NonSelfClosingTags_AppendsCorrectTag(string templateLine, string expectedTagName, string expectedId, string expectedClassName)
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine(templateLine));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.Append("<" + expectedTagName));
            if (!string.IsNullOrEmpty(expectedId))
                _classBuilderMock.Verify(x => x.AppendFormat(" id='{0}'", expectedId));
            if (!string.IsNullOrEmpty(expectedClassName))
                _classBuilderMock.Verify(x => x.AppendFormat(" class='{0}'", expectedClassName));
            _classBuilderMock.Verify(x => x.AppendFormat("</{0}>", expectedTagName));
        }

        [Test]
        public void Walk_SelfClosingTag_AppendsCorrectTag()
        {
            // Arrange
            const string tagName = "foo/";
            var tagNode = new HamlNodeTag(new HamlLine(tagName));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedTagName = "foo";
            _classBuilderMock.Verify(x => x.Append("<" + expectedTagName));
            _classBuilderMock.Verify(x => x.Append(" />"));
        }

        [Test]
        [TestCase(HtmlVersion.Html4, ">")]
        [TestCase(HtmlVersion.Html5, ">")]
        [TestCase(HtmlVersion.XHtml, " />")]
        public void Walk_AutoSelfClosingTag_AppendsCorrectTag(HtmlVersion htmlVersion, string expectedFormat)
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("br"));

            // Act
            _hamlOptions.HtmlVersion = htmlVersion;
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.Append(expectedFormat));
        }

        [Test]
        public void Walk_IndentedTag_AppendsIndent()
        {
            // Arrange
            const string indent = "  ";
            var tagNode = new HamlNodeTag(new HamlLine(indent + "p"));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.Append(indent));
        }

        [Test]
        public void Walk_NestedTags_AppendsCorrectTags()
        {
            // Arrange
            const string tagName = "p";
            const string nestedText = "Hello world";
            var tagNode = new HamlNodeTag(new HamlLine(tagName))
                              {
                                  new HamlNodeText(new HamlLine(nestedText))
                              };
            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.Append("<" + tagName));
            _classBuilderMock.Verify(x => x.Append(">"));
            _classBuilderMock.Verify(x => x.Append(nestedText));
            _classBuilderMock.Verify(x => x.AppendFormat("</{0}>", tagName));
        }

        [Test]
        public void Walk_IdFollowedByClassNodes_OrderedCorrectly()
        {
            // Arrange
            const string tagId = "id";
            const string tagClass = "class";
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeTagId(tagId),
                                  new HamlNodeTagClass(tagClass)
                              };

            int instrIndex = 0;
            _classBuilderMock.Setup(x => x.AppendFormat(" class='{0}'", It.IsAny<string>()))
                .Callback(() => Assert.That(instrIndex++ == 0));
            _classBuilderMock.Setup(x => x.AppendFormat(" id='{0}'", It.IsAny<string>()))
                .Callback(() => Assert.That(instrIndex++ == 1));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert - See Setup
        }

        [Test]
        public void Walk_ClassFollowedByIdNodes_OrderedCorrectly()
        {
            // Arrange
            const string tagId = "id";
            const string tagClass = "class";
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeTagClass(tagClass),
                                  new HamlNodeTagId(tagId)
                              };

            int instrIndex = 0;
            _classBuilderMock.Setup(x => x.AppendFormat(" class='{0}'", It.IsAny<string>()))
                .Callback(() => Assert.That(instrIndex++ == 0));
            _classBuilderMock.Setup(x => x.AppendFormat(" id='{0}'", It.IsAny<string>()))
                .Callback(() => Assert.That(instrIndex++ == 1));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert - See setup
        }

        [Test]
        public void Walk_MultipleClassNodes_WritesCorrectClassAttribute()
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeTagClass("class1"),
                                  new HamlNodeTagClass("class2")
                              };

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.AppendFormat(It.IsAny<string>(), "class1 class2"));
        }

        [Test]
        public void Walk_MultipleIdNodes_WritesCorrectIdAttribute()
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeTagId("id1"),
                                  new HamlNodeTagId("id2")
                              };

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.AppendFormat(It.IsAny<string>(), "id2"));
        }

    }
}
