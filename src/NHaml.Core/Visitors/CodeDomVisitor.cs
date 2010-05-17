using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using NHaml.Core.Ast;
using System.IO;

namespace NHaml.Core.Visitors
{
    class CodeDomVisitor : HtmlVisitor
    {
        protected CodeMemberMethod _code;
        protected Stack<string> _stack;
        protected string _writerName;
        protected int _blockCount;

        public CodeDomVisitor()
        {
            _code = new CodeMemberMethod();
            _stack = new Stack<string>();
            _writerName = "textWriter";
            _blockCount = 0;
        }

        protected override void WriteText(string text)
        {
            var writer = WriterCaller(_writerName);
            writer.Parameters.Add(new CodePrimitiveExpression { Value = text });
            _code.Statements.Add(new CodeExpressionStatement { Expression = writer });
        }

        protected override void WriteCode(string code, bool escapeHtml)
        {
            var writer = WriterCaller(_writerName);
            var toStringInvoke = ToStringCaller(code);
           
            if (escapeHtml)
            {
                var htmlEncodeInvoke = new CodeMethodInvokeExpression
                {
                    Method = new CodeMethodReferenceExpression
                    {
                        MethodName = "HtmlEncode",
                        TargetObject =
                            new CodeVariableReferenceExpression { VariableName = "HttpUtility" }
                    }
                };
                htmlEncodeInvoke.Parameters.Add(toStringInvoke);
                writer.Parameters.Add(htmlEncodeInvoke);
            }
            else
            {
                writer.Parameters.Add(toStringInvoke);
            }

            _code.Statements.Add(new CodeExpressionStatement { Expression = writer });
        }

        protected override void PushWriter()
        {
            _stack.Push(_writerName);
            _writerName = "textWriter" + _blockCount;
            _blockCount++;
            _code.Statements.Add(new CodeVariableDeclarationStatement
                {
                    Name = _writerName,
                    Type = new CodeTypeReference(typeof(StringWriter)),
                    InitExpression = new CodeObjectCreateExpression(typeof(StringWriter))
                });
        }

        protected override object PopWriter()
        {
            string varname = _writerName;
            _writerName = _stack.Pop();
            return varname;
        }

        protected override void WriteStartBlock(string code, bool hasChild)
        {
            throw new NotImplementedException();
        }

        protected override void WriteEndBlock()
        {
            throw new NotImplementedException();
        }

        protected override void WriteData(object data, string filter)
        {
            // TODO: filters
            var writer = WriterCaller(_writerName);
            var toStringInvoke = CodeWriterToStringCaller(data as string);
            writer.Parameters.Add(toStringInvoke);
            _code.Statements.Add(new CodeExpressionStatement { Expression = writer });
        }

        protected override LateBindingNode DataJoiner(string joinString, object[] data, bool sort)
        {
            // TODO
            return new LateBindingNode();
        }

        private CodeMethodInvokeExpression WriterCaller(string writername)
        {
            var writer = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = "Write",
                    TargetObject = new CodeVariableReferenceExpression
                    {
                        VariableName = writername
                    }
                }
            };
            return writer;
        }

        private CodeMethodInvokeExpression ToStringCaller(string parameter)
        {
            var result = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = "ToString",
                    TargetObject = new CodeVariableReferenceExpression
                    {
                        VariableName = "Convert"
                    }
                }
            };
            result.Parameters.Add(new CodeSnippetExpression { Value = parameter });
            return result;
        }

        private CodeMethodInvokeExpression ToStringCaller(CodeExpression parameter)
        {
            var result = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = "ToString",
                    TargetObject = new CodeVariableReferenceExpression
                    {
                        VariableName = "Convert"
                    }
                }
            };
            result.Parameters.Add(parameter);
            return result;
        }

        private CodeMethodInvokeExpression CodeWriterToStringCaller(string variable)
        {
            return ToStringCaller(new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = "GetStringBuilder",
                    TargetObject = new CodeVariableReferenceExpression
                    {
                        VariableName = variable
                    }
                }
            });
        }
    }
}
