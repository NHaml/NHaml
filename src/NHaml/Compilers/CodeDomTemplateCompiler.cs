using System;
using System.Text.RegularExpressions;
using NHaml.Exceptions;

namespace NHaml.Compilers
{
    public abstract class CodeDomTemplateCompiler : ITemplateCompiler
    {

        protected CodeDomTemplateCompiler(Regex lambdaRegex)
        {
            LambdaRegex = lambdaRegex;
        }

        public abstract TemplateClassBuilder CreateTemplateClassBuilder(string className, Type templateBaseType);

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var templateSource = templateParser.TemplateClassBuilder.Build(templateParser.TemplateEngine.Usings);
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

        protected Regex LambdaRegex { get; set; }


        public abstract string TranslateLambda(string codeLine, Match lambdaMatch);

        public abstract ITemplateTypeBuilder CreateTemplateTypeBuilder(TemplateEngine templateEngine);
    }
}