using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CSharp;

namespace NHaml.Compilers.CSharp2
{
    public class CSharp2TemplateCompiler : CodeDomTemplateCompiler
    {

        public CSharp2TemplateCompiler() : base(@"^(.+)(\(.*\))\s*=>\s*$")
        {
        }

        public override TemplateClassBuilder CreateTemplateClassBuilder(string className)
        {
            return new CSharp2TemplateClassBuilder(className);
        }

        protected override CodeDomProvider GetCodeDomProvider(Dictionary<string, string> dictionary)
        {
            return new CSharpCodeProvider(dictionary);
        }

        public override string TranslateLambda( string codeLine, Match lambdaMatch )
        {
            var part0 = codeLine.Substring( 0, lambdaMatch.Groups[1].Length - 2 );
            var part2 = lambdaMatch.Groups[2].Captures[0].Value;
            var part1 = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith( "()", StringComparison.OrdinalIgnoreCase ) ? null : ", ");
            return string.Format("{0}{1}delegate{2}{{", part0, part1, part2);
        }

        public override CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder(TemplateOptions options)
        {
            return new CSharp2TemplateTypeBuilder( options);
        }
    }
}
