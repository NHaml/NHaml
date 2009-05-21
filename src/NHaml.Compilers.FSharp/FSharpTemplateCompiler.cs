using System;
using System.Text.RegularExpressions;

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

        public override string TranslateLambda(string codeLine, Match lambdaMatch)
        {
            var methodBeingCalled = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
            var s = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
            var argDefinition = lambdaMatch.Groups[2].Captures[0].Value;
            return string.Format("{0}{1}{2} -> ", methodBeingCalled, s, argDefinition);
        }

        public override ITemplateTypeBuilder CreateTemplateTypeBuilder(TemplateEngine templateEngine)
        {
            return new FSharpTemplateTypeBuilder(templateEngine);
        }
    }
}