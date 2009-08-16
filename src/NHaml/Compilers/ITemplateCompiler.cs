using System;

namespace NHaml.Compilers
{
    public interface ITemplateCompiler
    {
        TemplateFactory Compile(TemplateParser templateParser, TemplateOptions options);
        BlockClosingAction RenderSilentEval(IViewSourceReader viewSourceReader, TemplateClassBuilder builder);
        TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType );

    }
}