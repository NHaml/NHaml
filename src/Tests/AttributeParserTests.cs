using System.Linq;
using NHaml.Exceptions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeParserTests
    {
        private static void AssertAttribute(AttributeParser parser, string name, string value, ParsedAttributeType type)
        {
            AssertAttribute(parser, null, name, value, type);
        }

        private static void AssertAttribute(AttributeParser parser, string schema, string name, string value,
                                            ParsedAttributeType type)
        {
            var attribute = parser.Attributes.FirstOrDefault(a => a.Name == name && a.Schema == schema);

            Assert.IsNotNull(attribute, string.Format("Attribute '{0}' not found.", name));

            Assert.AreEqual(schema, attribute.Schema, "Schema");
            Assert.AreEqual(value, attribute.Value, "Value");
            Assert.AreEqual(type, attribute.Type, "Type");
        }

        [Test]
        public void AttributeWithSchemaIsDifferent()
        {
            var parser = new AttributeParser("lang='en' xml:lang='en:us'");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, null, "lang", "en", ParsedAttributeType.String);
            AssertAttribute(parser, "xml", "lang", "en:us", ParsedAttributeType.String);
        }

        [Test]
        public void DoubleAndSingleQuotesEncoded()
        {
            var parser = new AttributeParser(@"b='a\'b\'' dd=""\""d\""e""");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "b", "a'b'", ParsedAttributeType.String);
            AssertAttribute(parser, "dd", "\"d\"e", ParsedAttributeType.String);
        }

        [Test]
        public void DoubleQuotes()
        {
            var parser = new AttributeParser("a=\"b\" cc=\"d\" eee=\"f\" ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", ParsedAttributeType.String);
            AssertAttribute(parser, "cc", "d", ParsedAttributeType.String);
            AssertAttribute(parser, "eee", "f", ParsedAttributeType.String);
        }

        [Test]
        public void Empty()
        {
            var parser = new AttributeParser("");

            parser.Parse();

            Assert.AreEqual(0, parser.Attributes.Count);
        }

        [Test]
        public void EncodingIsIgnoredIfItIsUsedAsStopper()
        {
            var parser = new AttributeParser(@" a=""abc\"" b='abc\' c=#{abc\} ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", @"abc\", ParsedAttributeType.String);
            AssertAttribute(parser, "b", @"abc\", ParsedAttributeType.String);
            AssertAttribute(parser, "c", @"abc\", ParsedAttributeType.Expression);
        }

        [Test]
        public void ExpressionInsideOfDoubleSingleQuotesAndEncodedQuotes()
        {
            var parser = new AttributeParser("a='#{\"a\"}' c=\"#{a}\"");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "a", "#{\"a\"}", ParsedAttributeType.String);
            AssertAttribute(parser, "c", "#{a}", ParsedAttributeType.String);
        }

        [Test]
        public void Expressions()
        {
            var parser = new AttributeParser("aaa=#{1+1} bb=#{\"t\"} c=#{f.ToString()} d=#{f=>\\{return 1\\}}");

            parser.Parse();

            Assert.AreEqual(4, parser.Attributes.Count);
            AssertAttribute(parser, "aaa", "1+1", ParsedAttributeType.Expression);
            AssertAttribute(parser, "bb", "\"t\"", ParsedAttributeType.Expression);
            AssertAttribute(parser, "c", "f.ToString()", ParsedAttributeType.Expression);
            AssertAttribute(parser, "d", @"f=>{return 1}", ParsedAttributeType.Expression);
        }

        [Test]
        public void OnlyReference()
        {
            var parser = new AttributeParser("a cc e");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", ParsedAttributeType.Reference);
            AssertAttribute(parser, "cc", "cc", ParsedAttributeType.Reference);
            AssertAttribute(parser, "e", "e", ParsedAttributeType.Reference);
        }

        [Test]
        public void ReferenceAsValue()
        {
            var parser = new AttributeParser("aa=b c=d eee=f ff=ff.aa ggg=ggg.bb.aa");

            parser.Parse();

            Assert.AreEqual(5, parser.Attributes.Count);
            AssertAttribute(parser, "aa", "b", ParsedAttributeType.Reference);
            AssertAttribute(parser, "c", "d", ParsedAttributeType.Reference);
            AssertAttribute(parser, "eee", "f", ParsedAttributeType.Reference);
            AssertAttribute(parser, "ff", "ff.aa", ParsedAttributeType.Reference);
            AssertAttribute(parser, "ggg", "ggg.bb.aa", ParsedAttributeType.Reference);
        }

        [Test]
        public void Schema()
        {
            var parser = new AttributeParser(@" a:b b:ccc='eee' eee:c=e ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", "b", ParsedAttributeType.Reference);
            AssertAttribute(parser, "b", "ccc", "eee", ParsedAttributeType.String);
            AssertAttribute(parser, "eee", "c", "e", ParsedAttributeType.Reference);
        }


        [Test]
        public void SingleQuotes()
        {
            var parser = new AttributeParser("a='b' c='d' ee='f'");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", ParsedAttributeType.String);
            AssertAttribute(parser, "c", "d", ParsedAttributeType.String);
            AssertAttribute(parser, "ee", "f", ParsedAttributeType.String);
        }

        [Test]
        public void SpacesBettwenKeyAndValue()
        {
            var parser = new AttributeParser("a =a bbb= b c = 'c'  dd  =  #{d} ");

            parser.Parse();

            Assert.AreEqual(4, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", ParsedAttributeType.Reference);
            AssertAttribute(parser, "bbb", "b", ParsedAttributeType.Reference);
            AssertAttribute(parser, "c", "c", ParsedAttributeType.String);
            AssertAttribute(parser, "dd", "d", ParsedAttributeType.Expression);
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnDoubleAttribute()
        {
            new AttributeParser(@" abc=a abc=b ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnDoubleAttributeWithDifferentCase()
        {
            new AttributeParser(@" abc=a AbC=b aBc=c ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnEmtpyAfterEqual()
        {
            new AttributeParser(@" a= ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenDoubleSingleQuoteClose()
        {
            new AttributeParser(@" a=""text ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenDoubleSingleQuoteOpen()
        {
            new AttributeParser(@" a=text"" ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenExpressionClose()
        {
            new AttributeParser(@" a=#{text ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenExpressionOpen()
        {
            new AttributeParser(@" a=text} ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenSingleQuoteClose()
        {
            new AttributeParser(@" a='text ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenSingleQuoteOpen()
        {
            new AttributeParser(@" a=text' ").Parse();
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnOnlyShema()
        {
            new AttributeParser(@" a: ").Parse();
        }
    }
}