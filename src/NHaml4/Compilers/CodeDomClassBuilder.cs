using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using System.Linq;

namespace NHaml4.Compilers.Abstract
{
    public class CodeDomClassBuilder : ITemplateClassBuilder
    {
        private const string TextWriterVariableName = "textWriter";

        public Type BaseType { get; set; }
        public int BlockDepth { get; set; }
        protected CodeMemberMethod RenderMethod { get; private set; }
        private int Depth { get; set; }
        private string ClassName { get; set; }

        public CodeDomClassBuilder()
        {
            RenderMethod = new CodeMemberMethod
                               {
                                   Name = "CoreRender",
                                   Attributes = MemberAttributes.Override | MemberAttributes.Family,
                               }
                               .WithParameter(typeof(TextWriter), "textWriter");
        }

        public void RenderEndBlock()
        {
            AppendCodeSnippet("}//");
        }

        protected void RenderBeginBlock()
        {
            AppendCodeSnippet("{//");
        }

        private IList<string> MergeRequiredImports(IEnumerable<string> imports)
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

        public void AppendAttributeNameValuePair(string name, IEnumerable<string> valueFragments, char quoteToUse)
        {
            string variableName = "value_" + RenderMethod.Statements.Count;
            RenderMethod.AddStatement(
                CodeDomFluentBuilder.GetDeclaration(typeof(StringBuilder), variableName,
                new CodeObjectCreateExpression("System.Text.StringBuilder", new CodeExpression[] { })));

            foreach (var fragment in valueFragments)
            {
                CodeExpression parameter;
                if (fragment.StartsWith("#{") && fragment.EndsWith("}"))
                {
                    parameter = CodeDomFluentBuilder.GetCodeMethodInvokeExpression("base.RenderValueOrKeyAsString")
                        .WithInvokePrimitiveParameter(fragment.Substring(2, fragment.Length-3));
                }
                else
                {
                    parameter = new CodePrimitiveExpression { Value = fragment };
                }

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

            ClassName = className;
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {

                var compileUnit = new CodeCompileUnit();

                var testNamespace = new CodeNamespace();
                compileUnit.Namespaces.Add(testNamespace);

                foreach (var import in imports)
                {
                    var namespaceImport = new CodeNamespaceImport(import);
                    testNamespace.Imports.Add(namespaceImport);
                }

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

        #region Will need this stuff one day!

        //public override void AppendSilentCode(string code)
        //{
        //    if (code == null) return;

        //    code = CommentMarkup + code.Trim();

        //    RenderMethod.Statements.Add(
        //        new CodeSnippetExpression { Value = code, });
        //}

        //public override void AppendCode(string code, bool escapeHtml)
        //{
        //    if (code == null) return;

        //    var write = CodeDomFluentBuilder
        //        .GetCodeMethodInvokeExpression("Write", CurrentTextWriterVariableName);
        //    var toStringInvoke = CodeDomFluentBuilder
        //        .GetCodeMethodInvokeExpression("ToString", "Convert")
        //        .WithCodeSnippetParameter(code);

        //    if (escapeHtml)
        //    {
        //        var htmlEncodeInvoke = CodeDomFluentBuilder
        //            .GetCodeMethodInvokeExpression("HtmlEncode", "HttpUtility")
        //            .WithCodeMethodParameter(toStringInvoke);

        //        write.Parameters.Add(htmlEncodeInvoke);
        //    }
        //    else
        //    {
        //        write.Parameters.Add(toStringInvoke);
        //    }

        //    RenderMethod.Statements.Add(
        //        new CodeExpressionStatement { Expression = write });
            
        //}

        //public override void AppendChangeOutputDepth(int depth)
        //{
        //    if (BlockDepth != depth)
        //    {

        //        var depthStatement = new CodeAssignStatement
        //                                 {
        //                                     Left = new CodePropertyReferenceExpression
        //                                                {
        //                                                    PropertyName = "Depth",
        //                                                    TargetObject =
        //                                                        new CodeVariableReferenceExpression
        //                                                            {
        //                                                                VariableName = "Output"
        //                                                            }
        //                                                },
        //                                     Right = new CodePrimitiveExpression
        //                                                 {
        //                                                     Value = depth
        //                                                 }
        //                                 };

        //        RenderMethod.Statements.Add(depthStatement);

        //        BlockDepth = depth;
        //    }
      
        //}

        //public override void AppendAttributeTokens(string schema, string name, IList<ExpressionStringToken> values)
        //{

        //    var _invoke1 = new CodeMethodInvokeExpression();
        //    _invoke1.Parameters.Add(new CodeVariableReferenceExpression
        //                                {
        //                                    VariableName = CurrentTextWriterVariableName
        //                                });

        //    _invoke1.Parameters.Add(new CodePrimitiveExpression
        //                                {
        //                                    Value = schema
        //                                });

        //    _invoke1.Parameters.Add(new CodePrimitiveExpression
        //                                {
        //                                    Value = name
        //                                });

        //    if (values.Count == 1)
        //    {
        //        var expressionStringToken = values[0];
        //        if (expressionStringToken.IsExpression)
        //        {
        //            _invoke1.Parameters.Add(new CodeSnippetExpression
        //                                        {
        //                                            Value = expressionStringToken.Value
        //                                        });
        //        }
        //        else
        //        {
        //            _invoke1.Parameters.Add(new CodePrimitiveExpression
        //                                        {
        //                                            Value = expressionStringToken.Value
        //                                        });
        //        }
        //    }
        //    else
        //    {
        //        var concatExpression = GetConcatExpression(values);
        //        _invoke1.Parameters.Add(concatExpression);
        //    }
        //
        //    _invoke1.Method = new CodeMethodReferenceExpression
        //                          {
        //                              MethodName = "RenderAttributeIfValueNotNull",
        //                              TargetObject = new CodeThisReferenceExpression()
        //                          };
        //    RenderMethod.Statements.Add(new CodeExpressionStatement { Expression = _invoke1 });
        //}

        //public static CodeMethodInvokeExpression GetConcatExpression(IList<ExpressionStringToken> values)
        //{
        //    var stringType = new CodeTypeReference(typeof(string));
        //    var arrayExpression = new CodeArrayCreateExpression
        //    {
        //        CreateType = new CodeTypeReference("System.String", 1)
        //        {
        //            ArrayElementType = stringType
        //        },
        //        Size = 0
        //    };


        //    foreach (var expressionStringToken in values)
        //    {
        //        if (expressionStringToken.IsExpression)
        //        {
        //            var toStringExpression = new CodeMethodInvokeExpression
        //            {
        //                Method = new CodeMethodReferenceExpression
        //                {
        //                    MethodName = "ToString",
        //                    TargetObject =
        //                        new CodeVariableReferenceExpression
        //                        {
        //                            VariableName = "Convert"
        //                        }
        //                }
        //            };

        //            toStringExpression.Parameters.Add(new CodeSnippetExpression
        //            {
        //                Value = expressionStringToken.Value
        //            });
        //            arrayExpression.Initializers.Add(toStringExpression);
        //        }
        //        else
        //        {
        //            var _value3 = new CodePrimitiveExpression
        //            {
        //                Value = expressionStringToken.Value
        //            };
        //            arrayExpression.Initializers.Add(_value3);
        //        }
        //    }
        //    var concatExpression = new CodeMethodInvokeExpression
        //    {
        //        Method = new CodeMethodReferenceExpression
        //        {
        //            MethodName = "Concat",
        //            TargetObject = new CodeTypeReferenceExpression
        //            {
        //                Type = stringType
        //            }
        //        }
        //    };
        //    concatExpression.Parameters.Add(arrayExpression);
        //    return concatExpression;
        //}
        //public override void AppendPreambleCode(string code)
        //{

        //    var expression = new CodeSnippetExpression
        //                         {
        //                             Value = code,
        //                         };

        //    //  RenderMethod.Statements.Insert(preambleCount, new CodeSnippetStatement(code));
        //    RenderMethod.Statements.Insert(PreambleCount, new CodeExpressionStatement(expression));
        //    PreambleCount++;
        //}
        #endregion
    }
}