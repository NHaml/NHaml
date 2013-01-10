using System.Collections.Generic;
using System.Web.NHaml.Compilers;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;
using System.Web.NHaml.TemplateBase;
using NUnit.Framework;

namespace NHaml.Tests.Compilers
{
    [TestFixture]
    public class CodeDomClassBuilder_Tests
    {
        const string ClassName = "Class1";

        [Test]
        public void Build_EmptyClassBuilder_ContainsCorrectUsingStatements()
        {
            var code = BuildEmptyClass();
            Assert.That(code, Is.StringContaining("using System;"));
            Assert.That(code, Is.StringContaining("using System.IO;"));
        }

        [Test]
        public void Build_EmptyClassBuilder_CreatesCorrectClass()
        {
            var code = BuildEmptyClass();
            Assert.That(code, Is.StringContaining("class " + ClassName));
        }

        [Test]
        public void Build_EmptyClassBuilder_InheritsFromCorrectClass()
        {
            var code = BuildEmptyClass();
            string expectedCode = ": " + typeof(Template).FullName;
            Assert.That(code, Is.StringContaining(expectedCode));
        }

        [Test]
        public void Build_EmptyClassBuilder_ContainsRenderMethod()
        {
            var code = BuildEmptyClass();
            string expectedCode = "protected override void CoreRender(System.IO.TextWriter textWriter)";
            Assert.That(code, Is.StringContaining(expectedCode));
        }

        private object BuildEmptyClass()
        {
            var classBuilder = new CodeDomClassBuilder();
            return classBuilder.Build(ClassName, typeof(Template),
                new List<string> { "System", "System.IO" });
        }

        [Test]
        public void Append_SingleLine_GeneratesWriteStatement()
        {
            string lineToWrite = "Test";
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.Append(lineToWrite);
            string result = classBuilder.Build(ClassName);

            Assert.That(result, Is.StringContaining("textWriter.Write(\"" + lineToWrite + "\");"));
        }

        [Test]
        public void AppendNewLine_NormalUse_GeneratesWriteStatement()
        {
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendNewLine();
            string result = classBuilder.Build(ClassName);

            Assert.That(result, Is.StringContaining("textWriter.WriteLine(\"\");"));
        }

        [Test]
        public void AppendCode_ValidCodeFragment_AppendsFragment()
        {
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendCodeToString("1+1");
            string result = classBuilder.Build(ClassName);

            Assert.That(result, Is.StringContaining("textWriter.Write(Convert.ToString(1+1));"));
        }

        [Test]
        public void AppendVariable_ValidVariableName_AppendsRenderValueOrKeyAsString()
        {
            const string variableName = "key";
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendVariable(variableName);

            string result = classBuilder.Build(ClassName);

            Assert.That(result, Is.StringContaining("textWriter.Write(RenderValueOrKeyAsString(\"" + variableName + "\"));"));
        }

        [Test]
        public void AppendSelfClosingTagSuffix_AppendsCorrectOutput()
        {
            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendSelfClosingTagSuffix();
            string result = classBuilder.Build(ClassName);
            Assert.That(result, Is.StringContaining("base.AppendSelfClosingTagSuffix()"));
        }

        [Test]
        public void AppendAttributeNameValuePair_TextLiteralHamlNode_AppendsRenderAttributeNameValuePair()
        {
            var classBuilder = new CodeDomClassBuilder();
            var valueNodes = new List<HamlNode> {new HamlNodeTextLiteral(-1, "value")};
            classBuilder.AppendAttributeNameValuePair("Name", valueNodes, '\"');
            string result = classBuilder.Build(ClassName);
            Assert.That(result, Is.StringContaining("= new System.Text.StringBuilder();"));
            Assert.That(result, Is.StringContaining("base.RenderAttributeNameValuePair(\"Name\", value_0.ToString(), '\\\"')"));
        }

        [Test]
        public void AppendAttributeNameValuePair_LiteralAndVariableHamlNode_BuildsValueCorrectly()
        {
            var classBuilder = new CodeDomClassBuilder();
            var valueFragments = new List<HamlNode>
                                     {
                                         new HamlNodeTextLiteral(-1, "value1"),
                                         new HamlNodeTextVariable(-1, "#{variable}")
                                     };
            classBuilder.AppendAttributeNameValuePair("Name", valueFragments, '\"');
            string result = classBuilder.Build(ClassName);
            Assert.That(result, Is.StringContaining(".Append(\"value1\");"));
            Assert.That(result, Is.StringContaining(".Append(base.RenderValueOrKeyAsString(\"variable\"));"));
        }

        [Test]
        public void AppendAttributeNameValuePair_ObjectReferenceHamlNode_BuildsValueCorrectly()
        {
            var classBuilder = new CodeDomClassBuilder();
            var valueFragments = new List<HamlNode>
                                     {
                                         new HamlNodeTextVariable(-1, "#{Model.Property}")
                                     };
            classBuilder.AppendAttributeNameValuePair("Name", valueFragments, '\"');
            string result = classBuilder.Build(ClassName);
            Assert.That(result, Is.StringContaining(".Append(Convert.ToString(Model.Property));"));
        }

        [Test]
        public void AppendCodeSnippet_NoChildNodes_BuildsValueCorrectly()
        {
            string codeStatement = "int c = 1";

            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendCodeSnippet(codeStatement, false);
            string result = classBuilder.Build(ClassName);
            Assert.That(result, Is.StringContaining(codeStatement + ";"));
        }

        [Test]
        public void AppendCodeSnippet_ChildNode_OpensCodeBlockCorrectly()
        {
            string codeStatement = "for (int c = 0; c < 10; c++)";

            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendCodeSnippet(codeStatement, true);
            string result = classBuilder.Build(ClassName);
            Assert.That(result, Is.StringContaining(codeStatement + "//;"));
        }

        [Test]
        public void AppendDocType_CallsTemplateMethodCorrectly()
        {
            const string docTypeId = "Transitional";

            var classBuilder = new CodeDomClassBuilder();
            classBuilder.AppendDocType(docTypeId);
            string result = classBuilder.Build(ClassName);

            const string expectedCodeStatement = "Write(GetDocType(\"" + docTypeId + "\"))";
            Assert.That(result, Is.StringContaining(expectedCodeStatement));
        }
    }
}
