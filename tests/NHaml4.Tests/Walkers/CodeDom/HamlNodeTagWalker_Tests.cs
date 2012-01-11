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
using NHaml4.Tests.Mocks;

namespace NHaml4.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTagWalker_Tests
    {
        private ClassBuilderMock _classBuilderMock;
        private HamlNodeTagWalker _tagWalker;
        private HamlOptions _hamlOptions;

        [SetUp]
        public void SetUp()
        {
            _classBuilderMock = new ClassBuilderMock();
            _hamlOptions = new HamlOptions();
            _tagWalker = new HamlNodeTagWalker(_classBuilderMock, _hamlOptions);
        }

        [Test]
        [TestCase("p", "<p></p>")]
        [TestCase("p#id", "<p id='id'></p>")]
        [TestCase("p.class", "<p class='class'></p>")]
        [TestCase("ns:id", "<ns:id></ns:id>")]
        public void Walk_NonSelfClosingTags_AppendsCorrectTag(string templateLine, string expectedOutput)
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine(templateLine));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedOutput));
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
            const string expectedTag = "<foo />";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedTag));
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
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedFormat));
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
            Assert.That(_classBuilderMock.Build(""), Is.StringStarting(indent));
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
            string expectedTag = string.Format("<{0}>{1}</{0}>", tagName, nestedText);
            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedTag));
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
            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedTag = @"<p class='class' id='id'></p>";
            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedTag));
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

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedTag = @"<p class='class' id='id'></p>";
            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedTag));
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
            const string expectedClassAttr = @"class='class1 class2'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedClassAttr));
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
            const string expectedIdAttr = @"id='id2'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedIdAttr));
        }

        [Test]
        public void Walk_IdHtmlAttribute_WritesCorrectIdAttribute()
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeHtmlAttributeCollection("(id='id')")
                              };

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedIdAttr = @"id='id'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedIdAttr));
        }


        [Test]
        public void Walk_ClassHtmlAttribute_WritesCorrectClassAttribute()
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeHtmlAttributeCollection("(class='class')")
                              };

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedIdAttr = @"class='class'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedIdAttr));
        }

        [Test]
        public void Walk_IdNoteAndIdHtmlAttribute_WritesCorrectIdAttribute()
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeTagId("id1"),
                                  new HamlNodeHtmlAttributeCollection("(id='id2')")
                              };

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedIdAttr = "id='id1_id2'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedIdAttr));
        }

        [Test]
        public void Walk_ClassNoteAndClassHtmlAttribute_WritesCorrectIdAttribute()
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("p"))
                              {
                                  new HamlNodeTagClass("class2"),
                                  new HamlNodeHtmlAttributeCollection("(class='class1')")
                              };

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedIdAttr = @"class='class1 class2'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedIdAttr));
        }

        [Test]
        [TestCase("p", "()", "<p></p>")]
        [TestCase("p/", "()", "<p />")]
        [TestCase("p", "(a='b')", "<p a='b'></p>")]
        public void Walk_EmptyAttributeCollectionNode_WritesCorrectAttributes(string tag, string attributes, string expectedOutput)
        {
            var tagNode = new HamlNodeTag(new HamlLine(tag))
            {
                new HamlNodeHtmlAttributeCollection(attributes)
            };

            _tagWalker.Walk(tagNode);

            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedOutput));
        }
    }
}
