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

        public TemplateFactory Compile(TemplateParser templateParser, TemplateOptions options)
        {
            var templateSource = templateParser.Builder.Build(options.Usings);

            var typeBuilder = new BooTemplateTypeBuilder(options);

           // Debug.WriteLine(templateSource);
            var templateType = typeBuilder.Build( templateSource, templateParser.Builder.ClassName );

            if( templateType == null )
            {
                var path = ListExtensions.Last(templateParser.ViewSources).Path;
                TemplateCompilationException.Throw( typeBuilder.CompilerResults,typeBuilder.Source, path );
            }

            return new TemplateFactory( templateType );
        }

        public BlockClosingAction RenderSilentEval(IViewSourceReader viewSourceReader, TemplateClassBuilder builder)
        {
            var code = viewSourceReader.CurrentInputLine.NormalizedText;

            var lambdaMatch = LambdaRegex.Match( code );

            var templateClassBuilder = (BooTemplateClassBuilder)builder;
            if( !lambdaMatch.Success )
            {
                templateClassBuilder.AppendSilentCode(code, !viewSourceReader.IsBlock);

                if( viewSourceReader.IsBlock )
                {
                    templateClassBuilder.BeginCodeBlock();

                    return templateClassBuilder.EndCodeBlock;
                }

                return MarkupRule.EmptyClosingAction;
            }

            var depth = viewSourceReader.CurrentInputLine.IndentCount;

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