using System;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml.Compilers.VisualBasic
{
    public class VisualBasicTemplateCompiler : ITemplateCompiler
    {
        private static readonly Regex LambdaRegex = new Regex(
            @"^(.+)(\(.*\))\s*=>\s*$",
            RegexOptions.Compiled | RegexOptions.Singleline );

        public TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType )
        {
            return new VisualBasicTemplateClassBuilder(className, templateBaseType);
        }

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var templateSource = templateParser.TemplateClassBuilder.Build();
            var typeBuilder = new VisualBasicTemplateTypeBuilder(templateParser.TemplateEngine);
            var templateType = typeBuilder.Build( templateSource, templateParser.TemplateClassBuilder.ClassName );

            if( templateType == null )
            {
                TemplateCompilationException.Throw( typeBuilder.CompilerResults,
                                                    typeBuilder.Source, templateParser.TemplatePath );
            }

            return new TemplateFactory( templateType );
        }

        public BlockClosingAction RenderSilentEval( TemplateParser templateParser )
        {
            var code = templateParser.CurrentInputLine.NormalizedText;

            var lambdaMatch = LambdaRegex.Match( code );

            if( !lambdaMatch.Success )
            {
                templateParser.TemplateClassBuilder
                    .AppendSilentCode( code, !templateParser.IsBlock );

                if( templateParser.IsBlock )
                {
                    templateParser.TemplateClassBuilder.BeginCodeBlock();

                    return () => templateParser.TemplateClassBuilder.EndCodeBlock();
                }

                return null;
            }

            var depth = templateParser.CurrentInputLine.IndentCount;

            code = TranslateLambda( code, lambdaMatch );

            templateParser.TemplateClassBuilder.AppendChangeOutputDepth( depth );
            templateParser.TemplateClassBuilder.AppendSilentCode( code, true );

            return () =>
                       {
                           templateParser.TemplateClassBuilder.AppendChangeOutputDepth( depth );
                           templateParser.TemplateClassBuilder.AppendSilentCode( "})", true );
                       };
        }




        public static string TranslateLambda(string codeLine, Match lambdaMatch)
        {
            var part2 = lambdaMatch.Groups[2].Captures[0].Value;
            var part0 = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
            var part1 = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
            return string.Format("{0}{1}{2} => {{", part0, part1, part2);
        }
    }
}