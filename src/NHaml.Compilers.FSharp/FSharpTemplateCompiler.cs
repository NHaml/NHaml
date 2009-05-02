using System;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml.Compilers.FSharp
{
    public class FSharpTemplateCompiler : ITemplateCompiler
    {
        private static readonly Regex LambdaRegex = new Regex(
          @"^(.+)(fun\(.*\))\s*->\s*$",
          RegexOptions.Compiled | RegexOptions.Singleline);

        public TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType )
        {
            return new FSharpTemplateClassBuilder( className, templateBaseType );
        }

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var templateSource = templateParser.TemplateClassBuilder.Build();
            var typeBuilder = new FSharpTemplateTypeBuilder( templateParser.TemplateEngine );
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

            var templateClassBuilder = (FSharpTemplateClassBuilder)templateParser.TemplateClassBuilder;
            if( !lambdaMatch.Success )
            {
                templateClassBuilder
                    .AppendSilentCode( code, !templateParser.IsBlock );

                if( templateParser.IsBlock )
                {
                    templateClassBuilder.BeginCodeBlock();

                    return templateClassBuilder.EndCodeBlock;
                }

                return null;
            }

            var depth = templateParser.CurrentInputLine.IndentCount;
            code = TranslateLambda(code, lambdaMatch);

            templateClassBuilder.AppendChangeOutputDepth( depth );
            templateClassBuilder.AppendSilentCode( code, true );

            return () =>
                       {
                           templateClassBuilder.AppendChangeOutputDepth( depth, false );
                           templateClassBuilder.EndCodeBlock();
                       };
        }

        public  string TranslateLambda(string codeLine, Match lambdaMatch)
        {
            var methodBeingCalled = codeLine.Substring(0, lambdaMatch.Groups[1].Length - 2);
            var s = (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith("()", StringComparison.OrdinalIgnoreCase) ? null : ", ");
            var argDefinition = lambdaMatch.Groups[2].Captures[0].Value;
            return string.Format("{0}{1}{2} -> ", methodBeingCalled, s, argDefinition);
        }
    }
}