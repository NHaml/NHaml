using System.Linq;
using NHaml.Exceptions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeParserTests
    {
        private static void AssertAttribute(AttributeParser parser, string name, string value, NHamlAttributeType type)
        {
            AssertAttribute(parser, null, name, value, type);
        }

        private static void AssertAttribute(AttributeParser parser, string schema, string name, string value,
                                            NHamlAttributeType type)
        {
            var attribute = parser.Attributes.FirstOrDefault(a => a.Name == name);

            Assert.IsNotNull(attribute, string.Format("Attribute '{0}' not found.", name));

            Assert.AreEqual(schema, attribute.Schema, "Schema");
            Assert.AreEqual(value, attribute.Value, "Value");
            Assert.AreEqual(type, attribute.Type, "Type");
        }

        [Test]
        public void DoubleAndSingleQuotesEncoded()
        {
            var parser = new AttributeParser(@"b='a\'b\'' dd=""\""d\""e""");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "b", "a\\'b\\'", NHamlAttributeType.String);
            AssertAttribute(parser, "dd", "\\\"d\\\"e", NHamlAttributeType.String);
        }

        [Test]
        public void DoubleQuotes()
        {
            var parser = new AttributeParser("a=\"b\" cc=\"d\" eee=\"f\" ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", NHamlAttributeType.String);
            AssertAttribute(parser, "cc", "d", NHamlAttributeType.String);
            AssertAttribute(parser, "eee", "f", NHamlAttributeType.String);
        }

        [Test]
        public void Empty()
        {
            var parser = new AttributeParser("");

            parser.Parse();

            Assert.AreEqual(0, parser.Attributes.Count);
        }

        [Test]
        public void ExpressionInsideOfDoubleSingleQuotesAndEncodedQuotes()
        {
            var parser = new AttributeParser("a='#{\"a\"}' c=\"#{a}\"");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "a", "#{\"a\"}", NHamlAttributeType.String);
            AssertAttribute(parser, "c", "#{a}", NHamlAttributeType.String);
        }

        [Test]
        public void Expressions()
        {
            var parser = new AttributeParser("aaa=#{1+1} bb=#{\"t\"} c=#{f.ToString()} d=#{f=>\\{return 1\\}}");

            parser.Parse();

            Assert.AreEqual(4, parser.Attributes.Count);
            AssertAttribute(parser, "aaa", "1+1", NHamlAttributeType.Expression);
            AssertAttribute(parser, "bb", "\"t\"", NHamlAttributeType.Expression);
            AssertAttribute(parser, "c", "f.ToString()", NHamlAttributeType.Expression);
            AssertAttribute(parser, "d", @"f=>\{return 1\}", NHamlAttributeType.Expression);
        }

        [Test]
        public void OnlyReference()
        {
            var parser = new AttributeParser("a cc e");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", NHamlAttributeType.Reference);
            AssertAttribute(parser, "cc", "cc", NHamlAttributeType.Reference);
            AssertAttribute(parser, "e", "e", NHamlAttributeType.Reference);
        }

        [Test]
        public void ReferenceAsValue()
        {
            var parser = new AttributeParser("aa=b c=d eee=f ff=ff.aa ggg=ggg.bb.aa");

            parser.Parse();

            Assert.AreEqual(5, parser.Attributes.Count);
            AssertAttribute(parser, "aa", "b", NHamlAttributeType.Reference);
            AssertAttribute(parser, "c", "d", NHamlAttributeType.Reference);
            AssertAttribute(parser, "eee", "f", NHamlAttributeType.Reference);
            AssertAttribute(parser, "ff", "ff.aa", NHamlAttributeType.Reference);
            AssertAttribute(parser, "ggg", "ggg.bb.aa", NHamlAttributeType.Reference);
        }

        [Test]
        public void Schema()
        {
            var parser = new AttributeParser(@" a:b b:ccc='eee' eee:c=e ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", "b", NHamlAttributeType.Reference);
            AssertAttribute(parser, "b", "ccc", "eee", NHamlAttributeType.String);
            AssertAttribute(parser, "eee", "c", "e", NHamlAttributeType.Reference);
        }


        [Test]
        public void SingleQuotes()
        {
            var parser = new AttributeParser("a='b' c='d' ee='f'");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", NHamlAttributeType.String);
            AssertAttribute(parser, "c", "d", NHamlAttributeType.String);
            AssertAttribute(parser, "ee", "f", NHamlAttributeType.String);
        }

        [Test]
        public void SpacesBettwenKeyAndValue()
        {
            var parser = new AttributeParser("a =a bbb= b c = 'c'  dd  =  #{d} ");

            parser.Parse();

            Assert.AreEqual(4, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", NHamlAttributeType.Reference);
            AssertAttribute(parser, "bbb", "b", NHamlAttributeType.Reference);
            AssertAttribute(parser, "c", "c", NHamlAttributeType.String);
            AssertAttribute(parser, "dd", "d", NHamlAttributeType.Expression);
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnEmtpyAfterEqual()
        {
            new AttributeParser(@" a= ").Parse();
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenDoubleSingleQuoteClose()
        {
            new AttributeParser(@" a=""text ").Parse();
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenDoubleSingleQuoteOpen()
        {
            new AttributeParser(@" a=text"" ").Parse();
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenExpressionClose()
        {
            new AttributeParser(@" a=#{text ").Parse();
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenExpressionOpen()
        {
            new AttributeParser(@" a=text} ").Parse();
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenSingleQuoteClose()
        {
            new AttributeParser(@" a='text ").Parse();
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenSingleQuoteOpen()
        {
            new AttributeParser(@" a=text' ").Parse();
        }

        [Test]
        [ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnOnlyShema()
        {
            new AttributeParser(@" a: ").Parse();
        }
    }
}