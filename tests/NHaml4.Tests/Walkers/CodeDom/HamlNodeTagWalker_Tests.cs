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
        [TestCase("p", "p", "")]
        [TestCase("p#id", "p", " id='id'")]
        [TestCase("ns:id", "ns:id", "")]
        public void Walk_NonSelfClosingTags_AppendsCorrectTag(string templateLine, string expectedTagName, string expectedAttributes)
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine(templateLine));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.AppendFormat("<{0}{1}>", expectedTagName, expectedAttributes));
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
            _classBuilderMock.Verify(x => x.AppendFormat("<{0}{1} />", expectedTagName, ""));
        }

        [Test]
        [TestCase(HtmlVersion.Html4, "<{0}{1}>")]
        [TestCase(HtmlVersion.Html5, "<{0}{1}>")]
        [TestCase(HtmlVersion.XHtml, "<{0}{1} />")]
        public void Walk_AutoSelfClosingTag_AppendsCorrectTag(HtmlVersion htmlVersion, string expectedFormat)
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("br"));

            // Act
            _hamlOptions.HtmlVersion = htmlVersion;
            _tagWalker.Walk(tagNode);

            // Assert
            _classBuilderMock.Verify(x => x.AppendFormat(expectedFormat, "br", ""));
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
            _classBuilderMock.Verify(x => x.AppendFormat("<{0}{1}>", tagName, ""));
            _classBuilderMock.Verify(x => x.Append(nestedText));
            _classBuilderMock.Verify(x => x.AppendFormat("</{0}>", tagName));
        }
    }
}
