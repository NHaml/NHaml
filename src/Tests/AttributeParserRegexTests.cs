using System.Text.RegularExpressions;
using NUnit.Framework;

namespace NHaml.Tests
{
    [TestFixture]
    public class AttributeParserRegexTests
    {
        [Test]
        public void StartCodeRegex()
        {
            var regex = AttributeParser.StartCodeRegex;
            Validate(regex, "a=#{", "=#{");
            Validate(regex, "a =#{", " =#{");
            Validate(regex, "a = #{", " = #{");
            Validate(regex, "a  =#{", "  =#{");
            Validate(regex, "a  =  #{", "  =  #{");
            Validate(regex, "a=  #{", "=  #{");
            Validate(regex, "a:b=#{", "=#{");
            Validate(regex, "a2:b2=#{", "=#{");
            Validate(regex, "a2=#{", "=#{");
            Validate(regex, ",a=#{", "=#{");


        }

        [Test]
        public void StartTextWithQuotesRegex()
        {
            var regex = AttributeParser.StartTextWithDoubleQuotesRegex;
            Validate(regex, "a=\"", "=\"");
            Validate(regex, "a =\"", " =\"");
            Validate(regex, "a = \"", " = \"");
            Validate(regex, "a= \"", "= \"");
            Validate(regex, "a  =\"", "  =\"");
            Validate(regex, "a  =  \"", "  =  \"");
            Validate(regex, "a:b=\"", "=\"");
            Validate(regex, "a2:b2=\"", "=\"");
            Validate(regex, "a2=\"", "=\"");
            Validate(regex, ",a=\"", "=\"");

        }
        [Test]
        public void StartTextWithOutQuotesRegex()
        {
            var regex = AttributeParser.StartTextWithOutQuotesRegex;
            Validate(regex, "a=", "=");
            Validate(regex, "a =", " =");
            Validate(regex, "a = ", " = ");
            Validate(regex, "a= ", "= ");
            Validate(regex, "a  =", "  =");
            Validate(regex, "a  =  ", "  =  ");
            Validate(regex, "a:b=", "=");
            Validate(regex, "a2:b2=", "=");
            Validate(regex, "a2=", "=");
            Validate(regex, ",a=", "=");

        }


        private void Validate(Regex regex, string input, string expected)
        {
            var match = regex.Match(input);
            var @group = match.Groups["ToTrim"];
            Assert.AreEqual(expected, @group.Value);
        }
    }
}