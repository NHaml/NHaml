using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.TemplateBase;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.IO;

namespace NHaml.IntegrationTests
{
    [TestFixture]
    public class CodeDomClassBuilder_IntegrationTests
    {
        const string ClassName = "Class1";

        [Test]
        public void AppendSelfClosingTagSuffix_XHtml4_CompilesValidTemplate()
        {
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendSelfClosingTagSuffix();
            string templateSource = classBuilder.Build(ClassName);
            var result = GenerateTemplateFromSource(templateSource);

            var writer = new StringWriter();
            result.Render(writer);
            Assert.That(writer.ToString(), Is.EqualTo(" />"));
        }

        [Test]
        public void AppendAttributeNameValuePair_XHtml4_CompilesValidTemplate()
        {
            var classBuilder = new CodeDomClassBuilder();
            var valueFragments = new List<HamlNode>
                                     {
                                         new HamlNodeTextLiteral(-1, "value"),
                                         new HamlNodeTextVariable(-1, "#{Variable}"),
                                         new HamlNodeTextLiteral(-1, "value")
                                     };
            classBuilder.AppendAttributeNameValuePair("name",
                valueFragments, '\"');
            string templateSource = classBuilder.Build(ClassName);
            var result = GenerateTemplateFromSource(templateSource);

            var dictionary = new Dictionary<string, object>
                                 {
                                     {"Variable", "Result"}
                                 };

            var writer = new StringWriter();
            result.Render(writer, HtmlVersion.XHtml, dictionary);
            Assert.That(writer.ToString(), Is.EqualTo(" name=\"valueResultvalue\""));
        }

        [Test]
        public void AppendMultipleAttributeNameValuePairs_XHtml4_CompilesValidTemplate()
        {
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendAttributeNameValuePair("name", new List<HamlNode> { new HamlNodeTextLiteral(-1, "value") }, '\"');
            classBuilder.AppendAttributeNameValuePair("name", new List<HamlNode> { new HamlNodeTextLiteral(-1, "value") }, '\"');
            string templateSource = classBuilder.Build(ClassName);
            var result = GenerateTemplateFromSource(templateSource);

            var writer = new StringWriter();
            result.Render(writer, HtmlVersion.XHtml);
            Assert.That(writer.ToString(), Is.EqualTo(" name=\"value\" name=\"value\""));
        }

        [Test]
        public void WriteNewLineIfRepeated_RepeatedCode_AppendsNewline()
        {
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendCodeSnippet("for (int c = 0; c < 3; c++)", true);
            classBuilder.Append("Test");
            classBuilder.RenderEndBlock();
            string templateSource = classBuilder.Build(ClassName);
            var result = GenerateTemplateFromSource(templateSource);

            var writer = new StringWriter();
            result.Render(writer, HtmlVersion.XHtml);
            Assert.That(writer.ToString(), Is.EqualTo("Test\r\nTest\r\nTest"));
        }

        private Template GenerateTemplateFromSource(string templateSource)
        {
            var typeBuilder = new CSharp2TemplateTypeBuilder();
            var templateCompiler = new CodeDomTemplateCompiler(typeBuilder);
            var templateFactory = templateCompiler.Compile(templateSource, ClassName, new List<string>());
            var result = templateFactory.CreateTemplate();
            return result;
        }

        private IList<Type> GetCompileTypes()
        {
            return new List<Type> {
                typeof(Template),
                typeof(System.Web.HttpUtility)
            };
        }
    }
}
