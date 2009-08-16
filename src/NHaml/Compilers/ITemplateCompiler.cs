using System;

namespace NHaml.Compilers
{
    public interface ITemplateCompiler
    {
        TemplateFactory Compile(IViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder);
        BlockClosingAction RenderSilentEval(IViewSourceReader viewSourceReader, TemplateClassBuilder builder);
        TemplateClassBuilder CreateTemplateClassBuilder( string className, Type templateBaseType );

    }
}