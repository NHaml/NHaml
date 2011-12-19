using System;
using System.Text.RegularExpressions;
using NHaml4.Compilers.CSharp2;
using NHaml;

namespace NHaml4.Compilers.CSharp3
{
    public  class CSharp3TemplateCompiler : CSharp2TemplateCompiler
    {
        public override string TranslateLambda( string codeLine, Match lambdaMatch )
        {
            var groups = lambdaMatch.Groups;
            var part2 = groups[2].Captures[0].Value;
            var part0 = codeLine.Substring( 0, groups[1].Length - 2 );
            var part1 = (groups[1].Captures[0].Value.Trim().EndsWith( "()", StringComparison.OrdinalIgnoreCase ) ? null : ", ");
            return string.Format("{0}{1}{2} => {{", part0, part1, part2);
        }


        public override CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder()
        {
            return new CSharp3TemplateTypeBuilder();
        }

    }
}