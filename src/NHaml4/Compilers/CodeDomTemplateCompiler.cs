using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml.Exceptions;
using NHaml4.Parser;
using NHaml;

namespace NHaml4.Compilers
{
    public abstract class CodeDomTemplateCompiler : ITemplateFactoryCompiler
    {
        private readonly Regex lambdaRegex;

        protected CodeDomTemplateCompiler(string lambdaRegex)
        {
            this.lambdaRegex = new Regex(lambdaRegex,
                RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public void Compile(string templateCode)
        {
        }


        public abstract TemplateClassBuilder CreateTemplateClassBuilder();

        public TemplateFactory Compile(HamlNode node, TemplateOptions options, TemplateClassBuilder builder)
        {
            //var typeBuilder = CreateTemplateTypeBuilder(options);
            ////TODO: leaky abstraction 
            //var classBuilder = (CodeDomClassBuilder) builder;
            //var provider = GetCodeDomProvider(typeBuilder.ProviderOptions);
            //classBuilder.CodeDomProvider = provider;
            //typeBuilder.CodeDomProvider = provider;
            //var templateSource = classBuilder.Build(options.Usings);
            
            //var templateType = typeBuilder.Build( templateSource, classBuilder.ClassName );

            //if( templateType == null )
            //{
            //    var viewSources = viewSourceReader.ViewSources;
            //    TemplateCompilationException.Throw(typeBuilder.CompilerResults, typeBuilder.Source, ListExtensions.Last(viewSources).Path);
            //}

            //return new TemplateFactory( templateType );
            return null;
        }

        protected abstract CodeDomProvider GetCodeDomProvider(Dictionary<string, string> dictionary);

        //public BlockClosingAction RenderSilentEval(HamlNode node, TemplateClassBuilder builder)
        //{
        //    var code = viewSourceReader.CurrentInputLine.NormalizedText;

        //    var lambdaMatch = lambdaRegex.Match(code);

        //    if (!lambdaMatch.Success)
        //    {
        //        builder.AppendSilentCode(code, !viewSourceReader.IsBlock);

        //        if (viewSourceReader.IsBlock)
        //        {
        //            builder.BeginCodeBlock();

        //            return builder.EndCodeBlock;
        //        }

        //        return MarkupRule.EmptyClosingAction;
        //    }

        //    var depth = viewSourceReader.CurrentInputLine.IndentCount;
        //    code = TranslateLambda(code, lambdaMatch);

        //    builder.AppendChangeOutputDepth(depth);
        //    builder.AppendSilentCode(code, true);

        //    return () =>
        //               {
        //                   builder.AppendChangeOutputDepth(depth);
        //                   builder.AppendSilentCode("})", true);
        //               };
        //    return null;
        //}

        public abstract string TranslateLambda(string codeLine, Match lambdaMatch);

        public abstract CodeDomTemplateTypeBuilder CreateTemplateTypeBuilder(TemplateOptions options);
    }
}