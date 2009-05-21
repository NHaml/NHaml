using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.FSharp.Compiler.CodeDom;

namespace NHaml.Compilers.FSharp
{
    public class FSharpTemplateCompiler : CodeDomTemplateCompiler
    {

        public FSharpTemplateCompiler()
            : base(@"^(.+)(fun\(.*\))\s*->\s*$")
        {
        }

        public override TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType )
        {
            return new FSharpTemplateClassBuilder( className, templateBaseType );
        }

        protected override CodeDomProvider GetCodeDomProvider(Dictionary<string, string> dictionary)
        {
            return new FSharpCodeProvider();
        }

        public override string TranslateLambda(string codeLine, Match lambdaMatch)
        {
            var methodBeingCalled = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
            var s = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
            var argDefinition = lambdaMatch.Groups[2].Captures[0].Value;
            return string.Format("{0}{1}{2} -> ", methodBeingCalled, s, argDefinition);
        }

        public override CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder(TemplateEngine templateEngine)
        {
            return new FSharpTemplateTypeBuilder(templateEngine);
        }
    }
}