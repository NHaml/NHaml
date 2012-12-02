using NHaml.Parser;
using NHaml.Walkers.CodeDom;
using NUnit.Framework;
using NHaml.IO;
using NHaml.Parser.Rules;
using NHaml.Tests.Mocks;

namespace NHaml.Tests.Walkers.CodeDom
{
    [TestFixture]
    public class HamlNodeTagWalker_Tests
    {
        private ClassBuilderMock _classBuilderMock;
        private HamlNodeTagWalker _tagWalker;
        private HamlHtmlOptions _hamlOptions;

        [SetUp]
        public void SetUp()
        {
            _classBuilderMock = new ClassBuilderMock();
            _hamlOptions = new HamlHtmlOptions();
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
            var tagNode = new HamlNodeTag(new HamlLine(templateLine, HamlRuleEnum.Tag, "", 0));

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
            var tagNode = new HamlNodeTag(new HamlLine(tagName, HamlRuleEnum.Tag, "", 0));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedTag = "<foo />";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedTag));
        }

        [Test]
        public void Walk_IndentedTag_AppendsIndent()
        {
            // Arrange
            const string indent = "  ";
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, indent, 0));

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
            var tagNode = new HamlNodeTag(new HamlLine(tagName, HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTextContainer(new HamlLine(nestedText, HamlRuleEnum.PlainText, "", 1, true)));
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
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTagId(0, tagId));
            tagNode.AddChild(new HamlNodeTagClass(0, tagClass));

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
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTagClass(0, tagClass));
            tagNode.AddChild(new HamlNodeTagId(0, tagId));

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
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTagClass(0, "class1"));
            tagNode.AddChild(new HamlNodeTagClass(0, "class2"));

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
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTagId(0, "id1"));
            tagNode.AddChild(new HamlNodeTagId(0, "id2"));

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
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeHtmlAttributeCollection(0, "(id='id')"));

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
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeHtmlAttributeCollection(0, "(class='class')"));

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
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTagId(0, "id1"));
            tagNode.AddChild(new HamlNodeHtmlAttributeCollection(0, "(id='id2')"));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedIdAttr = @"id='id1_id2'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedIdAttr));
        }

        [Test]
        public void Walk_ClassNoteAndClassHtmlAttribute_WritesCorrectIdAttribute()
        {
            // Arrange
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTagClass(0, "class2"));
            tagNode.AddChild(new HamlNodeHtmlAttributeCollection(0, "(class='class1')"));

            // Act
            _tagWalker.Walk(tagNode);

            // Assert
            const string expectedIdAttr = @"class='class1 class2'";
            Assert.That(_classBuilderMock.Build(""), Is.StringContaining(expectedIdAttr));
        }

        [Test]
        [TestCase("p", "()", "<p></p>")]
        [TestCase("p/", "()", "<p />")]
        [TestCase("p", "(a='b')", "<p a=\'b\'></p>")]
        public void Walk_EmptyAttributeCollectionNode_WritesCorrectAttributes(string tag, string attributes, string expectedOutput)
        {
            var tagNode = new HamlNodeTag(new HamlLine(tag, HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeHtmlAttributeCollection(0, attributes));

            _tagWalker.Walk(tagNode);

            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase("p", "p>", "", "<p><p></p></p>")]
        [TestCase("p<", "p", "", "<p><p></p></p>")]
        public void Walk_WhitespaceRemoval_GeneratesCorrectOutput(string line1, string line2, string line3, string expectedOutput)
        {
            var tagNode = new HamlNodeTag(new HamlLine(line1, HamlRuleEnum.Tag, "", 0));
            tagNode.AddChild(new HamlNodeTag(new HamlLine(line2, HamlRuleEnum.Tag, "  ", 0)));

            _tagWalker.Walk(tagNode);

            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Walk_InternalWhitespaceRemoval_GeneratesCorrectOutput()
        {
            var tagNode = new HamlNodeTag(new HamlLine("p<", HamlRuleEnum.Tag, "", 0));
            //tagNode.IsMultiLine = true;

            tagNode.AddChild(new HamlNodeTextContainer(new HamlLine("\n", HamlRuleEnum.PlainText, "", 0)));
            tagNode.AddChild(new HamlNodeTextContainer(new HamlLine("  Hello", HamlRuleEnum.PlainText, "", 0)));

            _tagWalker.Walk(tagNode);

            const string expectedOutput = "<p>Hello</p>";
            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedOutput));
        }

        [Test]
        public void Walk_InlineContent_GeneratesCorrectOutput()
        {
            var tagNode = new HamlNodeTag(new HamlLine("p", HamlRuleEnum.Tag));
            tagNode.AddChild(new HamlNodeTextContainer(new HamlLine("Content", HamlRuleEnum.PlainText, "\t", 0, true)));

            _tagWalker.Walk(tagNode);

            const string expectedOutput = "<p>Content</p>";
            Assert.That(_classBuilderMock.Build(""), Is.EqualTo(expectedOutput));
        }
    }
}
