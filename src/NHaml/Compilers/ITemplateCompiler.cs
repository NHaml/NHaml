using System;

namespace NHaml.Compilers
{
  public interface ITemplateCompiler
  {
    TemplateFactory Compile(TemplateParser templateParser);
    BlockClosingAction RenderSilentEval(TemplateParser templateParser);
    TemplateClassBuilder CreateTemplateClassBuilder(string className, Type templateBaseType);

    void RenderAttributes(TemplateParser templateParser, string attributes);
  }
}