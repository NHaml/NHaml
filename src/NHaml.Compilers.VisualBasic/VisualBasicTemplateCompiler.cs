using System;
using System.Text.RegularExpressions;

namespace NHaml.Compilers.VisualBasic
{
    public class VisualBasicTemplateCompiler : CodeDomTemplateCompiler
    {

        public VisualBasicTemplateCompiler()
            : base(@"^(.+)(\(.*\))\s*=>\s*$")
        {
        }


        public override TemplateClassBuilder CreateTemplateClassBuilder(string className, Type templateBaseType)
        {
            return new VisualBasicTemplateClassBuilder(className, templateBaseType);
        }

        public override string TranslateLambda(string codeLine, Match lambdaMatch)
        {
            var part2 = lambdaMatch.Groups[2].Captures[0].Value;
            var part0 = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
            var part1 = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
            return string.Format("{0}{1}{2} => {{", part0, part1, part2);
        }

        public override ITemplateTypeBuilder CreateTemplateTypeBuilder(TemplateEngine templateEngine)
        {
            return  new VisualBasicTemplateTypeBuilder(templateEngine);
        }
    }
}