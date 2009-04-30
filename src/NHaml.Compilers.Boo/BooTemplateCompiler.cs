using System;
using System.Text.RegularExpressions;

using NHaml.Exceptions;

namespace NHaml.Compilers.Boo
{
    public sealed class BooTemplateCompiler : ITemplateCompiler
    {
        public static readonly Regex LambdaRegex = new Regex(
          @"^(.+)(def\(.*\))\s*$",
          RegexOptions.Compiled | RegexOptions.Singleline );

        #region ITemplateCompiler Members

        public TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType )
        {
            return new BooTemplateClassBuilder( className, templateBaseType );
        }

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var templateSource = templateParser.TemplateClassBuilder.Build();

            //Console.WriteLine(templateSource);

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

            if( !lambdaMatch.Success )
            {
                templateParser.TemplateClassBuilder.AppendSilentCode( code, !templateParser.IsBlock );

                if( templateParser.IsBlock )
                {
                    templateParser.TemplateClassBuilder.BeginCodeBlock();

                    return () => templateParser.TemplateClassBuilder.EndCodeBlock();
                }

                return null;
            }

            var booTemplateClassBuilder = (BooTemplateClassBuilder)templateParser.TemplateClassBuilder;
            var depth = templateParser.CurrentInputLine.IndentCount;

            booTemplateClassBuilder.AppendChangeOutputDepth( depth, true );
            booTemplateClassBuilder.AppendSilentCode( code, false );
            booTemplateClassBuilder.BeginCodeBlock();

            return () =>
              {
                  booTemplateClassBuilder.AppendChangeOutputDepth( depth, false );
                  booTemplateClassBuilder.EndCodeBlock();
              };
        }



        #endregion
    }
}