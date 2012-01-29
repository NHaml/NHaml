using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace NHaml4.Compilers.Abstract
{
    public abstract class CodeDomClassBuilder : ITemplateClassBuilder
    {
        private const string DefaultTextWriterVariableName = "textWriter";
        private StringBuilder Output { get; set; }
        private StringBuilder Preamble { get; set; }

        private readonly CodeDomProvider _codeDomProvider;
        protected CodeMemberMethod RenderMethod{ get; private set; }

        protected abstract string CommentMarkup { get; }
        protected abstract void RenderEndBlock();
        protected abstract void RenderBeginBlock();

        private string CurrentTextWriterVariableName { get; set; }
        public Type BaseType { get; set; }
        private int Depth { get; set; }
        public int BlockDepth { get; set; }
        private string ClassName { get; set; }

        protected CodeDomClassBuilder(CodeDomProvider codeDomProvider)
        {
            _codeDomProvider = codeDomProvider;
            CurrentTextWriterVariableName = DefaultTextWriterVariableName;
            Output = new StringBuilder();
            Preamble = new StringBuilder();

            RenderMethod = new CodeMemberMethod
                               {
                                   Name = "CoreRender",
                                   Attributes = MemberAttributes.Override | MemberAttributes.Family,
                               };
            RenderMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(TextWriter), "textWriter"));
        }

        public void Append(string line)
        {
            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("Write", CurrentTextWriterVariableName)
                .WithPrimitiveParameter(line);

            RenderMethod.Statements.Add(
                new CodeExpressionStatement { Expression = writeInvoke });
        }

        public void AppendFormat(string content, params object[] args)
        {
            Append(string.Format(content, args));
        }

        public void AppendNewLine()
        {
            var writeInvoke = CodeDomFluentBuilder
                .GetCodeMethodInvokeExpression("WriteLine", CurrentTextWriterVariableName)
                .WithPrimitiveParameter("");

            RenderMethod.Statements.Add(
                new CodeExpressionStatement { Expression = writeInvoke });
        }

        public void BeginCodeBlock()
        {
            Depth++;

            RenderBeginBlock();
        }

        public void EndCodeBlock()
        {
            RenderEndBlock();
            Depth--;
        }

        public string Build(string className)
        {
            ClassName = className;
            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {

                var compileUnit = new CodeCompileUnit();

                var testNamespace = new CodeNamespace();
                compileUnit.Namespaces.Add(testNamespace);

                //foreach (var import in imports)
                //{
                //    var namespaceImport = new CodeNamespaceImport(import);
                //    testNamespace.Imports.Add(namespaceImport);

                //}

                var generator = _codeDomProvider.CreateGenerator(writer);
                var options = new CodeGeneratorOptions();
                var declaration = new CodeTypeDeclaration
                {
                    Name = className,
                    IsClass = true
                };
                declaration.BaseTypes.Add(new CodeTypeReference(typeof(NHaml4.TemplateBase.Template)));

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

    internal static class CodeDomFluentBuilder
    {
        public static CodeMethodInvokeExpression GetCodeMethodInvokeExpression(string methodName, string targetObject)
        {
            var result = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = methodName,
                    TargetObject =
                        new CodeVariableReferenceExpression { VariableName = targetObject }
                }
            };
            return result;
        }

        public static CodeMethodInvokeExpression WithPrimitiveParameter(this CodeMethodInvokeExpression expression, object parameter)
        {
            expression.Parameters.Add(
                new CodePrimitiveExpression { Value = parameter });
            return expression;
        }

        public static CodeMethodInvokeExpression WithCodeSnippetParameter(this CodeMethodInvokeExpression expression, string parameter)
        {
            expression.Parameters.Add(
                new CodeSnippetExpression { Value = parameter });
            return expression;
        }

        public static CodeMethodInvokeExpression WithCodeMethodParameter(this CodeMethodInvokeExpression expression, CodeMethodInvokeExpression parameter)
        {
            expression.Parameters.Add(parameter);
            return expression;
        }
    }
}