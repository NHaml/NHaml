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
    public class CSharp3ClassBuilder : CodeDomVisitorClassBuilder
    {
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

        CodeDomVisitor _visitor;
        CodeDomProvider _provider;
        CompilerParameters _options;
        CodeGeneratorOptions _generator;

        protected override CodeDomProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = new CSharpCodeProvider(new Dictionary<string, string>()
                        { 
                            {"CompilerVersion", "v3.5"}
                        }
                    );
                }
                return _provider;
            }
        }

        protected override CodeDomVisitor Visitor
        {
            get {
                if (_visitor == null)
                {
                    _visitor = new CSharpVisitor();
                }
                return _visitor;
            }
        }

        protected override CompilerParameters CompilerOptions
        {
            get {
                if (_options == null)
                {
                    _options = new CompilerParameters();
                }
                return _options;
            }
        }

        protected override CodeGeneratorOptions GeneratorOptions
        {
            get {
                if (_generator == null)
                {
                    _generator = new CodeGeneratorOptions();
                }
                return _generator;
            }
        }

        protected override bool SupportsDebug()
        {
            return true;
        }
    }
}
