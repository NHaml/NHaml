using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NHaml4.Parser;
using NHaml;
using System;
using NHaml4.Compilers.Abstract;

namespace NHaml4.Compilers
{
    public class CodeDomTemplateCompiler : ITemplateFactoryCompiler
    {
        //private readonly Regex lambdaRegex;
        private readonly CodeDomTemplateTypeBuilder _typeBuilder;

        public CodeDomTemplateCompiler(CodeDomTemplateTypeBuilder typeBuilder)
        {
            //this.lambdaRegex = new Regex(lambdaRegex,
            //    RegexOptions.Compiled | RegexOptions.Singleline);
            _typeBuilder = typeBuilder;
        }

        public TemplateFactory Compile(string templateSource, string className, IList<Type> references)
        {
            var templateType = _typeBuilder.Build( templateSource, className, references);

            //if( templateType == null )
            //{
            //    var viewSources = viewSourceReader.ViewSources;
            //    TemplateCompilationException.Throw(typeBuilder.CompilerResults, typeBuilder.Source, ListExtensions.Last(viewSources).Path);
            //}

            return new TemplateFactory( templateType );
        }

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

        //public abstract string TranslateLambda(string codeLine, Match lambdaMatch);
    }
}