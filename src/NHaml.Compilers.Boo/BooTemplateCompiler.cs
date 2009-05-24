using System;
using System.Text.RegularExpressions;

using NHaml.Exceptions;
using NHaml.Rules;

namespace NHaml.Compilers.Boo
{
    public sealed class BooTemplateCompiler : ITemplateCompiler
    {
        public static readonly Regex LambdaRegex = new Regex(
          @"^(.+)(def\(.*\))\s*$",
          RegexOptions.Compiled | RegexOptions.Singleline );

        public TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType )
        {
            return new BooTemplateClassBuilder( className, templateBaseType );
        }

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var templateSource = templateParser.TemplateClassBuilder.Build(templateParser.TemplateEngine.Usings);

            var typeBuilder = new BooTemplateTypeBuilder( templateParser.TemplateEngine );

           // Debug.WriteLine(templateSource);
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

            var templateClassBuilder = (BooTemplateClassBuilder)templateParser.TemplateClassBuilder;
            if( !lambdaMatch.Success )
            {
                templateClassBuilder.AppendSilentCode(code, !templateParser.IsBlock);

                if( templateParser.IsBlock )
                {
                    templateClassBuilder.BeginCodeBlock();

                    return templateClassBuilder.EndCodeBlock;
                }

                return MarkupRule.EmptyClosingAction;
            }

            var depth = templateParser.CurrentInputLine.IndentCount;

            templateClassBuilder.AppendChangeOutputDepth( depth, true );
            templateClassBuilder.AppendSilentCode( code, false );
            templateClassBuilder.BeginCodeBlock();

            return () =>
              {
                  templateClassBuilder.AppendChangeOutputDepth( depth, false );
                  templateClassBuilder.EndCodeBlock();
              };
        }
    }
}