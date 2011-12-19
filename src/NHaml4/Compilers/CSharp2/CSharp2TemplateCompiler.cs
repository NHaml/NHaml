using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using NHaml;

namespace NHaml4.Compilers.CSharp2
{
    public class CSharp2TemplateCompiler : CodeDomTemplateCompiler
    {

        public CSharp2TemplateCompiler()
            : base(@"^(.+)(\(.*\))\s*=>\s*$",new CSharp2TemplateClassBuilder(), new CSharp2TemplateTypeBuilder())
        {
        }

        public override string TranslateLambda( string codeLine, Match lambdaMatch )
        {
            var part0 = codeLine.Substring( 0, lambdaMatch.Groups[1].Length - 2 );
            var part2 = lambdaMatch.Groups[2].Captures[0].Value;
            var part1 = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith( "()", StringComparison.OrdinalIgnoreCase ) ? null : ", ");
            return string.Format("{0}{1}delegate{2}{{", part0, part1, part2);
        }
    }
}
