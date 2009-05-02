using System;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml.Compilers.CSharp2
{
    public class CSharp2TemplateCompiler : ITemplateCompiler
    {
        private static readonly Regex LambdaRegex = new Regex(
          @"^(.+)(\(.*\))\s*=>\s*$",
          RegexOptions.Compiled | RegexOptions.Singleline );


        public TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType )
        {
            return new CSharp2TemplateClassBuilder( className, templateBaseType );
        }

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var templateSource = templateParser.TemplateClassBuilder.Build();
            var typeBuilder = CreateTemplateTypeBuilder( templateParser.TemplateEngine );
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

                    return templateParser.TemplateClassBuilder.EndCodeBlock;
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

    

  

        public virtual string TranslateLambda( string codeLine, Match lambdaMatch )
        {
            return codeLine.Substring( 0, lambdaMatch.Groups[1].Length - 2 )
              + (lambdaMatch.Groups[1].Captures[0].Value.Trim().EndsWith( "()", StringComparison.OrdinalIgnoreCase ) ? null : ", ")
                + "delegate" + lambdaMatch.Groups[2].Captures[0].Value + "{";
        }

        internal virtual CSharp2TemplateTypeBuilder CreateTemplateTypeBuilder( TemplateEngine templateEngine )
        {
            return new CSharp2TemplateTypeBuilder( templateEngine );
        }
    }
}