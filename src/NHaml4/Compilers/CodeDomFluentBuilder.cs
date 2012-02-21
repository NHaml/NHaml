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

        public static CodeMethodInvokeExpression GetCodeMethodInvokeExpression(string methodName)
        {
            var result = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = methodName
                }
            };
            return result;
        }

        public static CodeVariableDeclarationStatement GetDeclaration(Type type, string name, CodeExpression valueExpression)
        {
            return new CodeVariableDeclarationStatement(type, name, valueExpression);
        }

        public static CodeMethodInvokeExpression WithParameter(this CodeMethodInvokeExpression expression, CodeExpression parameter)
        {
            expression.Parameters.Add(parameter);
            return expression;
        }

        public static CodeMethodInvokeExpression WithInvokePrimitiveParameter(this CodeMethodInvokeExpression expression, object parameter)
        {
            return WithParameter(expression, new CodePrimitiveExpression { Value = parameter });
        }

        public static CodeMethodInvokeExpression WithInvokeVariableReferenceParameter(this CodeMethodInvokeExpression expression, string variableName)
        {
            return WithParameter(expression, new CodeVariableReferenceExpression { VariableName = variableName });
        }

        public static CodeMethodInvokeExpression WithInvokeCodeSnippetToStringParameter(this CodeMethodInvokeExpression expression,
            string codeSnippet)
        {
            return WithParameter(expression, 
                GetCodeMethodInvokeExpression("ToString", "Convert").WithCodeSnippetParameter(codeSnippet));
        }

        public static CodeMethodInvokeExpression WithCodeSnippetParameter(this CodeMethodInvokeExpression expression,
            string codeSnippet)
        {
            return WithParameter(expression, new CodeSnippetExpression(codeSnippet));
        }

        public static CodeMethodInvokeExpression WithInvokeCodeParameter(this CodeMethodInvokeExpression expression,
            CodeMethodInvokeExpression expressionToInvoke)
        {
            return WithParameter(expression, expressionToInvoke);
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

        public static void AddStatement(this CodeMemberMethod method, CodeStatement statement)
        {
            method.Statements.Add(statement);
        }
    }
}
