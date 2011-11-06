using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using NHaml.Core.Visitors;
using NHaml.Core.Ast;
using NHaml.Core.Compilers;

using Microsoft.FSharp.Compiler.CodeDom;

namespace NHaml.Compilers.FSharp
{
    public class FSharpClassBuilder : CodeDomVisitorClassBuilder
    {
        internal class FSharpVisitor : CodeDomVisitor
        {
            private Regex _lambdaRegex;

            protected override CodeObject StartBlock { get { return new CodeSnippetStatement(" ("); } }
            protected override CodeObject EndBlock { get { return new CodeSnippetStatement(" )"); } }
            protected override CodeObject LambdaEndBlock { get { return new CodeSnippetExpression("})"); } }
            protected override bool SupportLambda { get { return true; } }

            protected override Regex LambdaRegex
            {
                get
                {
                    if (_lambdaRegex == null)
                    {
                        _lambdaRegex = new Regex(@"^(.+)(fun\(.*\))\s*->\s*$", RegexOptions.Compiled | RegexOptions.Singleline);
                    }
                    return _lambdaRegex;
                }
            }

            protected override string TranslateLambda( string codeLine, Match lambdaMatch )
            {
                var methodBeingCalled = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
                var s = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
                var argDefinition = lambdaMatch.Groups[2].Captures[0].Value;
                return string.Format("{0}{1}{2} -> ", methodBeingCalled, s, argDefinition);
            }

            protected override string Comment
            {
                get { return String.Empty; }
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
                    _provider = new FSharpCodeProvider();
                }
                return _provider;
            }
        }

        protected override CodeDomVisitor Visitor
        {
            get {
                if (_visitor == null)
                {
                    _visitor = new FSharpVisitor();
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

        public override string GenerateSource(NHaml.Core.Template.TemplateOptions options)
        {
            string code = base.GenerateSource(options);
            //WTF??
            code = code.Replace("|> ignore", string.Empty);
            code = code.Replace(@"
                                                                                                                ", string.Empty);
            return code;
        }

        public override Type GenerateType(NHaml.Core.Template.TemplateOptions options)
        {
            AddReferences(options);
            CompilerOptions.GenerateInMemory = true;
            CompilerOptions.IncludeDebugInformation = false;
            string code = GenerateSource(options);
            CompilerResults = Provider.CompileAssemblyFromSource(CompilerOptions, code);
            if (ContainsErrors())
            {
                return null;
            }
            var assembly = CompilerResults.CompiledAssembly;
            return ExtractType(ClassName, assembly);
        }
    }
}

