using System;

namespace NHaml.BackEnds
{
  public interface ICompilerBackEnd
  {
    IAttributeRenderer AttributeRenderer { get; }
    ISilentEvalRenderer SilentEvalRenderer { get; }
    ITemplateClassBuilder CreateTemplateClassBuilder(Type viewBaseType, string className);
    Type BuildView(CompilationContext compilationContext);
  }
}