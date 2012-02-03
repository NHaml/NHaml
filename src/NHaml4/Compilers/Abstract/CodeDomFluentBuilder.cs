using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;

namespace NHaml4.Compilers.Abstract
{
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

        public static CodeExpressionStatement GetExpressionStatement(CodeMethodInvokeExpression writeInvoke)
        {
            return new CodeExpressionStatement { Expression = writeInvoke };
        }

        public static CodeMemberMethod WithParameter(this CodeMemberMethod method, Type type, string name)
        {
            method.Parameters.Add(new CodeParameterDeclarationExpression(type, name));
            return method;
        }

        public static void AddExpressionStatement(this CodeMemberMethod method, CodeMethodInvokeExpression expression)
        {
            method.Statements.Add(CodeDomFluentBuilder.GetExpressionStatement(expression));
        }
    }
}
