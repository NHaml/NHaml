
namespace NHaml.Compilers
{
    public interface ITemplateCompiler
    {
        TemplateFactory Compile(ViewSourceReader viewSourceReader, TemplateOptions options, TemplateClassBuilder builder);
        BlockClosingAction RenderSilentEval(ViewSourceReader viewSourceReader, TemplateClassBuilder builder);
        TemplateClassBuilder CreateTemplateClassBuilder( string className );

    }
}