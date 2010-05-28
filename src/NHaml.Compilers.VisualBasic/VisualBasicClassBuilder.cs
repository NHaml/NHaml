using System;
using System.Collections.Generic;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using NHaml.Core.Visitors;
using NHaml.Core.Ast;
using NHaml.Core.Compilers;

using Microsoft.VisualBasic;

namespace NHaml.Compilers.VisualBasic
{
    public class VisualBasicClassBuilder : CodeDomVisitorClassBuilder
    {
        internal class VisualBasicVisitor : CodeDomVisitor
        {
            private Regex _lambdaRegex;

            protected override CodeObject StartBlock { get { return null; } }
            protected override CodeObject EndBlock { get { return null; } }
            protected override CodeObject LambdaEndBlock { get { return null; } }
            protected override bool SupportLambda { get { return true; } }

            protected override Regex LambdaRegex
            {
                get
                {
                    if (_lambdaRegex == null)
                    {
                        _lambdaRegex = new Regex(@"^(.+)(\(.*\))\s*=>\s*$", RegexOptions.Compiled | RegexOptions.Singleline);
                    }
                    return _lambdaRegex;
                }
            }

            protected override string TranslateLambda( string codeLine, Match lambdaMatch )
            {
                var part2 = lambdaMatch.Groups[2].Captures[0].Value;
                var part0 = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
                var part1 = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
                return string.Format("{0}{1}{2} => {{", part0, part1, part2);
            }

            protected override string Comment
            {
                get { return "'"; }
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
                    _provider = new VBCodeProvider(new Dictionary<string, string>()
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
                    _visitor = new VisualBasicVisitor();
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

