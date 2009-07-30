using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml.Exceptions;
using NHaml.Rules;

namespace NHaml.Compilers
{
    public abstract class CodeDomTemplateCompiler : ITemplateCompiler
    {

        private readonly Regex lambdaRegex;

        protected CodeDomTemplateCompiler(string lambdaRegex)
        {
            this.lambdaRegex = new Regex(lambdaRegex,
                RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public abstract TemplateClassBuilder CreateTemplateClassBuilder(string className, Type templateBaseType);

        public TemplateFactory Compile( TemplateParser templateParser )
        {
            var typeBuilder = CreateTemplateTypeBuilder( templateParser.TemplateEngine );
            //TODO: leaky abstraction 
            var classBuilder = (CodeDomClassBuilder) templateParser.TemplateClassBuilder;
            var provider = GetCodeDomProvider(typeBuilder.ProviderOptions);
            classBuilder.CodeDomProvider = provider;
            typeBuilder.CodeDomProvider = provider;
            var templateSource = classBuilder.Build( templateParser.TemplateEngine.Options.Usings );
            
            var templateType = typeBuilder.Build( templateSource, classBuilder.ClassName );

            if( templateType == null )
            {
                TemplateCompilationException.Throw( typeBuilder.CompilerResults,
                                                    typeBuilder.Source, templateParser.TemplateViewSource.Path);
            }

            return new TemplateFactory( templateType );
        }

        protected abstract CodeDomProvider GetCodeDomProvider(Dictionary<string, string> dictionary);

        public BlockClosingAction RenderSilentEval( TemplateParser templateParser )
        {
            var code = templateParser.CurrentInputLine.NormalizedText;

            var lambdaMatch = lambdaRegex.Match( code );

            if( !lambdaMatch.Success )
            {
                templateParser.TemplateClassBuilder
                    .AppendSilentCode( code, !templateParser.IsBlock );

                if( templateParser.IsBlock )
                {
                    templateParser.TemplateClassBuilder.BeginCodeBlock();

                    return templateParser.TemplateClassBuilder.EndCodeBlock;
                }

                return MarkupRule.EmptyClosingAction;
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



        public abstract string TranslateLambda(string codeLine, Match lambdaMatch);

        public abstract CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder(TemplateEngine templateEngine);
    }
}