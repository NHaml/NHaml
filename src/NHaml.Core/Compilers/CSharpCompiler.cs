using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Core.Visitors;
using NHaml.Core.Ast;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;

namespace NHaml.Core.Compilers
{
    public class CSharpCompiler
    {
        private CodeDomVisitor Visitor;

        public CSharpCompiler(DocumentNode n)
        {
            Visitor = new CSharpVisitor();
            Visitor.Visit(n);
        }

        public string GetSource()
        {
            CodeMemberMethod m = Visitor.Method;
            CSharpCodeProvider provider = new CSharpCodeProvider();
            StringWriter sw = new StringWriter();
            IndentedTextWriter tw = new IndentedTextWriter(sw);
            provider.GenerateCodeFromMember(m, tw, new CodeGeneratorOptions());
            return sw.GetStringBuilder().ToString();
        }

        internal class CSharpVisitor : CodeDomVisitor
        {
            protected override string StartBlock
            {
                get
                {
                    return "{//";
                }
            }

            protected override string EndBlock
            {
                get
                {
                    return "}//";
                }
            }
        }
    }
}
