using System.Linq;
using NHaml.Exceptions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeParser2Tests
    {
        private static void AssertAttribute(AttributeParser2 parser, string name, string value, ParsedAttributeType type)
        {
            AssertAttribute(parser, null, name, value, type);
        }

        private static void AssertAttribute(AttributeParser2 parser, string schema, string name, string value,
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
            var parser = new AttributeParser2("lang='en' xml:lang='en:us'");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, null, "lang", "en", ParsedAttributeType.String);
            AssertAttribute(parser, "xml", "lang", "en:us", ParsedAttributeType.String);
        }
        
        [Test]
        public void CurlyBracesInsideCode()
        {
            var parser = new AttributeParser2(@"action = #{Convert.ToString(new { ID=5\})}");

            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "action", "Convert.ToString(new { ID=5})", ParsedAttributeType.Expression);
        }

        [Test]
        public void ExpressionWithoutHash()
        {
            var parser = new AttributeParser2(@"src = Html.Content().AppRoot() ");

            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "src", "Html.Content().AppRoot()", ParsedAttributeType.Reference);
        }

        [Test]
        public void DoubleAndSingleQuotesEncoded()
        {
            var parser = new AttributeParser2(@"b='a\'b\'' dd=""\""d\""e""");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "b", "a'b'", ParsedAttributeType.String);
            AssertAttribute(parser, "dd", "\"d\"e", ParsedAttributeType.String);
        }

        [Test]
        public void DoubleQuotes()
        {
            var parser = new AttributeParser2("a=\"b\" cc=\"d\" eee=\"f\" ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", ParsedAttributeType.String);
            AssertAttribute(parser, "cc", "d", ParsedAttributeType.String);
            AssertAttribute(parser, "eee", "f", ParsedAttributeType.String);
        }

        [Test]
        public void Empty()
        {
            var parser = new AttributeParser2("");

            parser.Parse();

            Assert.AreEqual(0, parser.Attributes.Count);
        }

        [Test]
        public void EncodingIsIgnoredIfItIsUsedAsStopper()
        {
            var parser = new AttributeParser2(@" a=""abc\\"" b='abc\\' c=#{abc\\} ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", @"abc\", ParsedAttributeType.String);
            AssertAttribute(parser, "b", @"abc\", ParsedAttributeType.String);
            AssertAttribute(parser, "c", @"abc\", ParsedAttributeType.Expression);
        }

        [Test]
        public void ExpressionInsideOfDoubleSingleQuotesAndEncodedQuotes()
        {
            var parser = new AttributeParser2("a='#{\"a\"}' c=\"#{a}\"");

            parser.Parse();

            Assert.AreEqual(2, parser.Attributes.Count);
            AssertAttribute(parser, "a", "#{\"a\"}", ParsedAttributeType.String);
            AssertAttribute(parser, "c", "#{a}", ParsedAttributeType.String);
        }

        [Test]
        public void Expressions()
        {
            var parser = new AttributeParser2("aaa=#{1+1} bb=#{\"t\"} c=#{f.ToString()} d=#{f=>\\{return 1\\}}");

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
            var parser = new AttributeParser2("a cc e");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", ParsedAttributeType.String);
            AssertAttribute(parser, "cc", "cc", ParsedAttributeType.String);
            AssertAttribute(parser, "e", "e", ParsedAttributeType.String);
        }

        [Test]
        public void SimpleOnlyReference()
        {
            var parser = new AttributeParser2("a");

            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", ParsedAttributeType.String);
        }


        [Test]
        public void SimpleOnlyReferenceWithSpace()
        {
            var parser = new AttributeParser2("a ");

            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "a", "a", ParsedAttributeType.String);
        }

        [Test]
        public void ReferenceAsValue()
        {
            var parser = new AttributeParser2("aa=b c=d eee=f ff=ff.aa ggg=ggg.bb.aa");

            parser.Parse();

            Assert.AreEqual(5, parser.Attributes.Count);
            AssertAttribute(parser, "aa", "b", ParsedAttributeType.Reference);
            AssertAttribute(parser, "c", "d", ParsedAttributeType.Reference);
            AssertAttribute(parser, "eee", "f", ParsedAttributeType.Reference);
            AssertAttribute(parser, "ff", "ff.aa", ParsedAttributeType.Reference);
            AssertAttribute(parser, "ggg", "ggg.bb.aa", ParsedAttributeType.Reference);
        }


        [Test]
        public void SimpleReference()
        {
            var parser = new AttributeParser2("a=bb");

            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "a", "bb", ParsedAttributeType.Reference);
        }

        [Test]
        public void Schema()
        {
            var parser = new AttributeParser2(@" a:b b:ccc='eee' eee:c=e ");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", "b", ParsedAttributeType.String);
            AssertAttribute(parser, "b", "ccc", "eee", ParsedAttributeType.String);
            AssertAttribute(parser, "eee", "c", "e", ParsedAttributeType.Reference);
        }


        [Test]
        public void SingleQuotes()
        {
            var parser = new AttributeParser2("a='b' c='d' ee='f'");

            parser.Parse();

            Assert.AreEqual(3, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", ParsedAttributeType.String);
            AssertAttribute(parser, "c", "d", ParsedAttributeType.String);
            AssertAttribute(parser, "ee", "f", ParsedAttributeType.String);
        }

        [Test]
        public void SimpleSingleQuotes()
        {
            var parser = new AttributeParser2("a='b'");

            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "a", "b", ParsedAttributeType.String);
        }

        [Test]
        public void SpacesBettwenKeyAndValue()
        {
            var parser = new AttributeParser2("a =a bbb= b c = 'c'  dd  =  #{d} ");

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
            new AttributeParser2(@" abc=a abc=b ").Parse();
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
            new AttributeParser2(@" a=""text ").Parse();
        }

        [Test]
        public void DoubleQuoteOpen()
        {
            var parser = new AttributeParser2(@" a=text"" ");
            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "a", "text\"", ParsedAttributeType.Reference);
        }

        [Test]
        public void SingleQuoteOpen()
        {
            var parser = new AttributeParser2(@" a=text' ");
            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "a", "text'", ParsedAttributeType.Reference);
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenExpressionClose()
        {
            new AttributeParser2(@" a=#{text ").Parse();
        }
        [Test]
        public void ValueEndsWithCurly()
        {
            var parser = new AttributeParser2(@" a=text} ");
            parser.Parse();

            Assert.AreEqual(1, parser.Attributes.Count);
            AssertAttribute(parser, "a", "text}", ParsedAttributeType.Reference);
        }

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnForgottenSingleQuoteClose()
        {
            new AttributeParser(@" a='text ").Parse();
        }

 

        [Test, ExpectedException(typeof (SyntaxException))]
        public void ThrowExceptionOnOnlyShema()
        {
            new AttributeParser2(@" a: ").Parse();
        }
    }
}