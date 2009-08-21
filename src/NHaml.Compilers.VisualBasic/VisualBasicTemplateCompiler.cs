using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace NHaml.Compilers.VisualBasic
{
    public class VisualBasicTemplateCompiler : CodeDomTemplateCompiler
    {

        public VisualBasicTemplateCompiler()
            : base(@"^(.+)(\(.*\))\s*=>\s*$")
        {
        }


        public override TemplateClassBuilder CreateTemplateClassBuilder(string className)
        {
            return new VisualBasicTemplateClassBuilder(className);
        }

        protected override CodeDomProvider GetCodeDomProvider(Dictionary<string, string> dictionary)
        {
            return new VBCodeProvider(dictionary);
        }

        public override string TranslateLambda(string codeLine, Match lambdaMatch)
        {
            var part2 = lambdaMatch.Groups[2].Captures[0].Value;
            var part0 = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
            var part1 = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
            return string.Format("{0}{1}{2} => {{", part0, part1, part2);
        }

        public override CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder(TemplateOptions options)
        {
            return  new VisualBasicTemplateTypeBuilder(options);
        }
    }
}