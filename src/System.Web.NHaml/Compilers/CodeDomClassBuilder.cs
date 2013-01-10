using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.NHaml.Parser;
using System.Web.NHaml.Parser.Rules;
using Microsoft.CSharp;

namespace System.Web.NHaml.Compilers
{
    public class CodeDomClassBuilder : ITemplateClassBuilder
    {
        private const string TextWriterVariableName = "textWriter";

        private CodeMemberMethod RenderMethod { get; set; }

        public CodeDomClassBuilder()
        {
            // ReSharper disable BitwiseOperatorOnEnumWihtoutFlags
            RenderMethod = new CodeMemberMethod
                               {
                                   Name = "CoreRender",
                                   Attributes = MemberAttributes.Override | MemberAttributes.Family,
                               }
                               .WithParameter(typeof(TextWriter), "textWriter");
            // ReSharper restore BitwiseOperatorOnEnumWihtoutFlags
        }

        public void RenderEndBlock()
        {
            AppendCodeSnippet("}//");
        }

        private void RenderBeginBlock()
        {
            AppendCodeSnippet("{//");
        }

        private IEnumerable<string> MergeRequiredImports(IEnumerable<string> imports)
        {
            var result = new List<string>(imports);
            if (result.Contains("System") == false)
                result.Add("System");
            return result;
        }

        public void Append(string line)
        {
            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokePrimitiveParameter(line);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        public void AppendFormat(string content, params object[] args)
        {
            Append(string.Format(content, args));
        }

        public void AppendNewLine()
        {
            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("WriteLine", TextWriterVariableName)
                .WithInvokePrimitiveParameter("");

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        public void AppendCodeToString(string code)
        {
            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeSnippetToStringParameter(code);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        public void AppendCodeSnippet(string code, bool containsChildren)
        {
            if (containsChildren)
            {
                if (IsCodeBlockBeginningElse(code) == false)
                    InitialiseCodeBlock();
                RenderMethod.Statements.Add(
                    new CodeSnippetExpression { Value = code + "//"});
                RenderBeginBlock();
                WriteNewLineIfRepeated();
            }
            else
            {
                AppendCodeSnippet(code);
            }
        }

        private bool IsCodeBlockBeginningElse(string code)
        {
            return (code.ToLower().Trim() + " ").StartsWith("else ");
        }

        private void InitialiseCodeBlock()
        {
            AppendCodeSnippet("HasCodeBlockRepeated = false;");
        }

        private void WriteNewLineIfRepeated()
        {
            AppendCodeSnippet("WriteNewLineIfRepeated(textWriter)");
        }

        public void AppendDocType(string docTypeId)
        {
            var docType = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("GetDocType")
                .WithInvokePrimitiveParameter(docTypeId);

            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeParameter(docType);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        private void AppendCodeSnippet(string code)
        {
            RenderMethod.Statements.Add(
                new CodeSnippetExpression { Value = code });
        }

        public void AppendVariable(string variableName)
        {
            var renderValueOrKeyAsString = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("RenderValueOrKeyAsString")
                .WithInvokePrimitiveParameter(variableName);

            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeParameter(renderValueOrKeyAsString);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        //public void BeginCodeBlock()
        //{
        //    Depth++;
        //    RenderBeginBlock();
        //}

        //public void EndCodeBlock()
        //{
        //    RenderEndBlock();
        //    Depth--;
        //}

        public void AppendAttributeNameValuePair(string name, IEnumerable<HamlNode> valueFragments, char quoteToUse)
        {
            string variableName = "value_" + RenderMethod.Statements.Count;
            RenderMethod.AddStatement(
                CodeDomFluentBuilder.GetDeclaration(typeof(StringBuilder), variableName,
                new CodeObjectCreateExpression("System.Text.StringBuilder", new CodeExpression[] { })));

            foreach (var fragment in valueFragments)
            {
                CodeExpression parameter;
                if (fragment is HamlNodeTextVariable)
                {
                    string nodeVariableName = ((HamlNodeTextVariable) fragment).VariableName;
                    if (nodeVariableName.All(ch => Char.IsLetterOrDigit(ch)))
                    parameter = CodeDomFluentBuilder.GetCodeMethodInvokeExpression("base.RenderValueOrKeyAsString")
                        .WithInvokePrimitiveParameter(nodeVariableName);
                    else
                    {
                        parameter = CodeDomFluentBuilder.GetCodeMethodInvokeExpression("ToString", "Convert")
                            .WithCodeSnippetParameter(nodeVariableName);
                    }
                }
                else
                {
                    parameter = new CodePrimitiveExpression { Value = fragment.Content };
                }

            //var writeInvoke = CodeDomFluentBuilder
            //    .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
            //    .WithInvokeCodeSnippetToStringParameter(code);

            //RenderMethod.AddExpressionStatement(writeInvoke);
        


                RenderMethod.AddExpressionStatement(
                    CodeDomFluentBuilder.GetCodeMethodInvokeExpression("Append", variableName)
                    .WithParameter(parameter));
            }

            var outputExpression = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("base.RenderAttributeNameValuePair")
                .WithInvokePrimitiveParameter(name)
                .WithParameter(CodeDomFluentBuilder.GetCodeMethodInvokeExpression("ToString", variableName))
                .WithInvokePrimitiveParameter(quoteToUse);

            RenderMethod.AddExpressionStatement(
                CodeDomFluentBuilder.GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithParameter(outputExpression));
        }

        public void AppendSelfClosingTagSuffix()
        {
            var renderValueOrKeyAsString = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("base.AppendSelfClosingTagSuffix");

            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("Write", TextWriterVariableName)
                .WithInvokeCodeParameter(renderValueOrKeyAsString);

            RenderMethod.AddExpressionStatement(writeInvoke);
        }

        public string Build(string className)
        {
            return Build(className, typeof(TemplateBase.Template), new List<string>());
        }

        public string Build(string className, Type baseType, IEnumerable<string> imports)
        {
            imports = MergeRequiredImports(imports);

            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {

                var compileUnit = new CodeCompileUnit();

                var testNamespace = new CodeNamespace();
                compileUnit.Namespaces.Add(testNamespace);

                testNamespace.Imports.AddRange(
                    imports.Select(x => new CodeNamespaceImport(x)).ToArray());

                var generator = new CSharpCodeProvider().CreateGenerator(writer);
                var options = new CodeGeneratorOptions();
                var declaration = new CodeTypeDeclaration
                {
                    Name = className,
                    IsClass = true
                };
                declaration.BaseTypes.Add(new CodeTypeReference(baseType));

                declaration.Members.Add(RenderMethod);

                testNamespace.Types.Add(declaration);
                generator.GenerateCodeFromNamespace(testNamespace, writer, options);

                //TODO: implement IDisposable
                writer.Close();
            }

            return builder.ToString();
        }

        public void Clear()
        {
            RenderMethod.Statements.Clear();
        }
    }
}