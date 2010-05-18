using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using NHaml.Core.Ast;
using System.IO;

namespace NHaml.Core.Visitors
{
    public abstract class CodeDomVisitor : HtmlVisitor
    {
        private CodeMemberMethod _code;
        private Stack<string> _stack;
        private string _writerName;
        private int _blockCount;

        public CodeDomVisitor()
        {
            _code = new CodeMemberMethod();
            _code.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        new CodeTypeReference(typeof(TextWriter)),
                        "textWriter"
                    )
                );
            _stack = new Stack<string>();
            _writerName = "textWriter";
            _blockCount = 0;
        }

        public CodeMemberMethod Method { get { return _code; } }

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
            _code.Statements.Add(new CodeSnippetStatement(code));
            if (hasChild) _code.Statements.Add(new CodeSnippetStatement(StartBlock));
        }

        protected override void WriteEndBlock()
        {
            _code.Statements.Add(new CodeSnippetStatement(EndBlock));
        }

        protected abstract string StartBlock { get; }
        protected abstract string EndBlock { get; }

        protected override void WriteData(object data, string filter)
        {
            var writer = WriterCaller(_writerName);
            var toStringInvoke = CodeWriterToStringCaller(data as string);
            if (filter == null)
            {
                writer.Parameters.Add(toStringInvoke);
            } else {
                switch (filter)
                {
                    case "preserve":
                        {
                            var invoke = new CodeMethodInvokeExpression
                                    {
                                        Method = new CodeMethodReferenceExpression(toStringInvoke,"Replace"),
                                    };
                            invoke.Parameters.Add(
                                new CodeFieldReferenceExpression(
                                    new CodeVariableReferenceExpression("Environment"),"NewLine")
                                );
                            invoke.Parameters.Add(new CodePrimitiveExpression("&#x000A;"));
                            var converter = new CodeBinaryOperatorExpression(
                                invoke,
                                CodeBinaryOperatorType.Add,
                                new CodePrimitiveExpression("&#x000A;"));
                            writer.Parameters.Add(converter);
                            break;
                        }
                    case "plain":
                        {
                            writer.Parameters.Add(toStringInvoke);
                            break;
                        }
                    case "escaped":
                        {
                            var converter = new CodeMethodInvokeExpression
                            {
                                Method = new CodeMethodReferenceExpression
                                {
                                    MethodName = "HtmlEncode",
                                    TargetObject = new CodeVariableReferenceExpression
                                    {
                                        VariableName = "HttpUtility"
                                    }
                                }
                            };
                            converter.Parameters.Add(toStringInvoke);
                            writer.Parameters.Add(converter);
                            break;
                        }
                    default:
                        throw new NotSupportedException();
                }
            }
            _code.Statements.Add(new CodeExpressionStatement { Expression = writer });
        }

        protected override LateBindingNode DataJoiner(string joinString, object[] data, bool sort)
        {
            string djWriterName = "textWriter" + _blockCount;
            _blockCount++;

            _code.Statements.Add(new CodeVariableDeclarationStatement
            {
                Name = djWriterName,
                Type = new CodeTypeReference(typeof(StringWriter)),
                InitExpression = new CodeObjectCreateExpression(typeof(StringWriter))
            });
            _code.Statements.Add(new CodeVariableDeclarationStatement
            {
                Name = djWriterName+"l",
                Type = new CodeTypeReference(typeof(List<string>)),
                InitExpression = new CodeObjectCreateExpression(typeof(List<string>))
            });

            foreach (object o in data)
            {
                var adder = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression(djWriterName + "l"),
                        "Add")
                    );
                var atostring = ToStringCaller(o as string);
                adder.Parameters.Add(atostring);
                _code.Statements.Add(adder);
            }
            if (sort)
            {
                var sorter = new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            new CodeVariableReferenceExpression(djWriterName+"l"),
                            "Sort"
                        )
                    );
                _code.Statements.Add(sorter);
            }
            var joincall = new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression("String"),
                        "Join"
                    )
                );
            joincall.Parameters.Add(new CodePrimitiveExpression(joinString));
            joincall.Parameters.Add(new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodeVariableReferenceExpression(djWriterName+"l"),
                        "ToArray")
                    )
                );
            var writerCaller = WriterCaller(djWriterName);
            writerCaller.Parameters.Add(joincall);
            _code.Statements.Add(writerCaller);
            return new LateBindingNode() { Value = djWriterName };
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
