using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using NHaml.Core.Ast;
using System.IO;
using NHaml.Core.Template;

namespace NHaml.Core.Visitors
{
    public class CodeDomHtmlHelperPartialMethod : IPartialRenderMethod
    {
        public object RenderMethod(string PartialName, object PartialObject)
        {
            var result = new CodeMethodInvokeExpression
            {
                Method = new CodeMethodReferenceExpression
                {
                    MethodName = "RenderPartial",
                    TargetObject = new CodeVariableReferenceExpression
                    {
                        VariableName = "Html"
                    }
                }
            };
            result.Parameters.Add(new CodePrimitiveExpression(PartialName));
            if (PartialObject != null)
            {
                result.Parameters.Add(new CodeSnippetExpression { Value = PartialObject.ToString() });
            }
            return result;
        }
    }

    public abstract class CodeDomVisitor : HtmlVisitor
    {
        private CodeMemberMethod _actualCode;
        private CodeMemberMethod _containsContent;
        private CodeMemberMethod _runContent;
        private Dictionary<string, CodeMemberMethod> _methods;
        private Stack<string> _stack;
        private StringBuilder _builder;
        private string _writerName;
        private int _blockCount;
        private IPartialRenderMethod _partialMethod;

        private CodeMemberMethod CreateNewMethod(string name)
        {
            var Code = new CodeMemberMethod();
            Code.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        new CodeTypeReference(typeof(TextWriter)),
                        "textWriter"
                    )
                );
            Code.Name = "Render"+name;
            Code.Attributes = MemberAttributes.Public;
            _methods.Add(name, Code);

            var containsCode = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("name"),
                    CodeBinaryOperatorType.ValueEquality,
                    new CodePrimitiveExpression(name)
                ),
                new CodeMethodReturnStatement(new CodePrimitiveExpression(true))
            );
            _containsContent.Statements.Add(containsCode);

            var runCode = new CodeConditionStatement(
                new CodeBinaryOperatorExpression(
                    new CodeVariableReferenceExpression("name"),
                    CodeBinaryOperatorType.ValueEquality,
                    new CodePrimitiveExpression(name)
                ),
                new CodeExpressionStatement(
                    new CodeMethodInvokeExpression(
                        new CodeThisReferenceExpression(),
                        "Render"+name,
                        new CodeVariableReferenceExpression("textWriter")
                    )
                )
            );
            _runContent.Statements.Add(runCode);
            return Code;
        }

        public CodeDomVisitor()
        {
            _methods = new Dictionary<string, CodeMemberMethod>();
            _stack = new Stack<string>();
            _writerName = "textWriter";
            _blockCount = 0;
            _builder = new StringBuilder();

            _containsContent = new CodeMemberMethod();
            _containsContent.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        new CodeTypeReference(typeof(string)),
                        "name"
                    )
                );
            _containsContent.ReturnType = new CodeTypeReference(typeof(bool));
            _containsContent.Name = "ContainsContent";
            _containsContent.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            _runContent = new CodeMemberMethod();
            _runContent.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        new CodeTypeReference(typeof(TextWriter)),
                        "textWriter"
                    )
                );
            _runContent.Parameters.Add(
                    new CodeParameterDeclarationExpression(
                        new CodeTypeReference(typeof(string)),
                        "name"
                    )
                );
            _runContent.Name = "RunContent";
            _runContent.Attributes = MemberAttributes.Public | MemberAttributes.Override;

            _partialMethod = Options.GetPartialRenderMethod();
        }

        public CodeDomVisitor(TemplateOptions options) : this()
        {
            Options = options;
        }

        public Dictionary<string,CodeMemberMethod> Methods { get { return _methods; } }
        public CodeMemberMethod ContainsContentMethod { get { return _containsContent; } }
        public CodeMemberMethod RunContentMethod { get { return _runContent; } }

        public virtual void RenderPartial(string PartialName, string Code)
        {
            var result = (CodeExpression)_partialMethod.RenderMethod(PartialName, Code);
            _actualCode.Statements.Add(result);
        }

        public override void Visit(MetaNode node)
        {
            PopString();
            if (node.Name == "contentplaceholder")
            {
                bool hasChild = node.Child != null;
                var childInvoke = new CodeMethodInvokeExpression
                {
                    Method = new CodeMethodReferenceExpression
                    {
                        MethodName = "RunContent",
                        TargetObject = new CodeVariableReferenceExpression
                        {
                            VariableName = "Child"
                        }
                    }
                };
                childInvoke.Parameters.Add(new CodeVariableReferenceExpression { VariableName = _writerName });
                childInvoke.Parameters.Add(new CodePrimitiveExpression { Value = node.Value });

                if (hasChild)
                {
                    var baseInvoke = new CodeMethodInvokeExpression
                    {
                        Method = new CodeMethodReferenceExpression
                        {
                            MethodName = "RunContent",
                            TargetObject = new CodeThisReferenceExpression()
                        }
                    };
                    baseInvoke.Parameters.Add(new CodeVariableReferenceExpression { VariableName = _writerName });
                    baseInvoke.Parameters.Add(new CodePrimitiveExpression { Value = node.Value });

                    var ifStatement = new CodeConditionStatement(
                            new CodeMethodInvokeExpression(
                                new CodeVariableReferenceExpression("Child"),
                                "ContainsContent",
                                new CodePrimitiveExpression(node.Value)
                            )
                        );
                    ifStatement.TrueStatements.Add(childInvoke);
                    ifStatement.FalseStatements.Add(baseInvoke);
                    _actualCode.Statements.Add(ifStatement);

                    string oldMethod = "Main";
                    foreach (var pair in _methods)
                    {
                        if (pair.Value == _actualCode)
                            oldMethod = pair.Key;
                    }
                    _actualCode = CreateNewMethod(node.Value);
                    if (node.Child != null)
                        Visit(node.Child);
                    PopString();
                    _actualCode = _methods[oldMethod];
                }
                else
                {
                    _actualCode.Statements.Add(childInvoke);
                }
            }
            else if (node.Name == "partialcontent")
            {
                string obj = null;
                foreach (var attr in node.Attributes)
                {
                    if (attr.Name == "model")
                    {
                        obj = ((TextChunk)(((TextNode)attr.Value).Chunks[0])).Text;
                    }
                }
                RenderPartial(node.Value,obj);
            }
            else if (node.Name == "content")
            {
                string oldMethod = "Main";
                foreach (var pair in _methods) {
                    if (pair.Value == _actualCode)
                        oldMethod = pair.Key;
                }
                _actualCode = CreateNewMethod(node.Value);
                if (node.Child != null)
                    Visit(node.Child);
                PopString();
                _actualCode = _methods[oldMethod];
            }
            else
            {
                base.Visit(node);
            }
        }

        protected override void StartVisit(DocumentNode node)
        {
            if (node.Metadata.ContainsKey("content"))
            {
                _actualCode = CreateNewMethod("BaseContentHolderMethod");
            }
            else
            {
                _actualCode = CreateNewMethod("Main");
            }
            base.StartVisit(node);
        }

        protected override void EndVisit(DocumentNode node)
        {
            PopString();
            _containsContent.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(false)));
            base.EndVisit(node);
        }

        protected void PushString(string text)
        {
            _builder.Append(text);
        }

        protected void PopString()
        {
            if (_builder.Length > 0)
            {
                var writer = WriterCaller(_writerName);
                writer.Parameters.Add(new CodePrimitiveExpression { Value = _builder.ToString() });
                _actualCode.Statements.Add(new CodeExpressionStatement { Expression = writer });
                _builder = new StringBuilder();
            }
        }

        protected override void WriteText(string text)
        {
            PushString(text);
        }

        protected override void WriteCode(string code, bool escapeHtml)
        {
            PopString();
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

            _actualCode.Statements.Add(new CodeExpressionStatement { Expression = writer });
        }

        protected override void PushWriter()
        {
            PopString();
            _stack.Push(_writerName);
            _writerName = "textWriter" + _blockCount;
            _blockCount++;
            _actualCode.Statements.Add(new CodeVariableDeclarationStatement
                {
                    Name = _writerName,
                    Type = new CodeTypeReference(typeof(StringWriter)),
                    InitExpression = new CodeObjectCreateExpression(typeof(StringWriter))
                });
        }

        protected override object PopWriter()
        {
            PopString();
            string varname = _writerName;
            _writerName = _stack.Pop();
            return varname;
        }

        protected override void WriteStartBlock(string code, bool hasChild)
        {
            PopString();
            _actualCode.Statements.Add(new CodeSnippetStatement(code));
            if (hasChild) _actualCode.Statements.Add(new CodeSnippetStatement(StartBlock));
        }

        protected override void WriteEndBlock()
        {
            PopString();
            _actualCode.Statements.Add(new CodeSnippetStatement(EndBlock));
        }

        protected abstract string StartBlock { get; }
        protected abstract string EndBlock { get; }

        protected override void WriteData(object data, string filter)
        {
            PopString();
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
            _actualCode.Statements.Add(new CodeExpressionStatement { Expression = writer });
        }

        protected override LateBindingNode DataJoiner(string joinString, object[] data, bool sort)
        {
            PopString();
            if (data.Length > 1)
            {
                string djWriterName = "textWriter" + _blockCount;
                _blockCount++;

                _actualCode.Statements.Add(new CodeVariableDeclarationStatement
                {
                    Name = djWriterName,
                    Type = new CodeTypeReference(typeof(StringWriter)),
                    InitExpression = new CodeObjectCreateExpression(typeof(StringWriter))
                });
                _actualCode.Statements.Add(new CodeVariableDeclarationStatement
                {
                    Name = djWriterName + "l",
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
                    _actualCode.Statements.Add(adder);
                }
                if (sort)
                {
                    var sorter = new CodeMethodInvokeExpression(
                            new CodeMethodReferenceExpression(
                                new CodeVariableReferenceExpression(djWriterName + "l"),
                                "Sort"
                            )
                        );
                    _actualCode.Statements.Add(sorter);
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
                            new CodeVariableReferenceExpression(djWriterName + "l"),
                            "ToArray")
                        )
                    );
                var writerCaller = WriterCaller(djWriterName);
                writerCaller.Parameters.Add(joincall);
                _actualCode.Statements.Add(writerCaller);
                return new LateBindingNode() { Value = djWriterName };
            }
            else
            {
                return new LateBindingNode() { Value = data[0] as string };
            }
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
