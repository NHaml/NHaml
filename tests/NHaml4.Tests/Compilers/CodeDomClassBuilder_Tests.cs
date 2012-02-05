using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NHaml4.Compilers.Abstract;

namespace NHaml4.Tests.Compilers
{
    [TestFixture]
    public class CodeDomClassBuilder_Tests
    {
        const string ClassName = "Class1";

        [TestFixture]
        public class Build_EmptyClassBuilder
        {
            private string _code;

            [SetUp]
            public void Setup()
            {
                var classBuilder = new CodeDomClassBuilder(
                    new List<string> { "System", "System.IO" }
                );
                _code = classBuilder.Build(ClassName);
            }

            [Test]
            public void ContainsCorrectUsingStatements()
            {
                Assert.That(_code, Is.StringContaining("using System;"));
                Assert.That(_code, Is.StringContaining("using System.IO;"));
            }

            [Test]
            public void CreatesCorrectClass()
            {
                Assert.That(_code, Is.StringContaining("class " + ClassName));
            }

            [Test]
            public void InheritsFromCorrectClass()
            {
                string expectedCode = ": " + typeof(NHaml4.TemplateBase.Template).FullName;
                Assert.That(_code, Is.StringContaining(expectedCode));
            }

            [Test]
            public void ContainsRenderMethod()
            {
                string expectedCode = "protected override void CoreRender(System.IO.TextWriter textWriter)";
                Assert.That(_code, Is.StringContaining(expectedCode));
            }
        }

        [Test]
        public void Append_SingleLine_GeneratesWriteStatement()
        {
            string lineToWrite = "Test";
            var classBuilder = new CodeDomClassBuilder(new List<string>());
            classBuilder.Append(lineToWrite);
            string result = classBuilder.Build(ClassName);

            Assert.That(result, Is.StringContaining("textWriter.Write(\"" + lineToWrite + "\");"));
        }

        [Test]
        public void AppendNewLine_NormalUse_GeneratesWriteStatement()
        {
            var classBuilder = new CodeDomClassBuilder(new List<string>());
            classBuilder.AppendNewLine();
            string result = classBuilder.Build(ClassName);

            Assert.That(result, Is.StringContaining("textWriter.WriteLine(\"\");"));
        }

        [Test]
        public void AppendCode_ValidCodeFragment_AppendsFragment()
        {
            var classBuilder = new CodeDomClassBuilder(new List<string>());
            classBuilder.AppendCode("1+1");
            string result = classBuilder.Build(ClassName);

            Assert.That(result, Is.StringContaining("textWriter.Write(Convert.ToString(1+1));"));
        }
    }
}
