using System;

namespace NHaml.BackEnds
{
  public interface ICompilerBackEnd
  {
    IAttributeRenderer AttributeRenderer { get; }
    ISilentEvalRenderer SilentEvalRenderer { get; }
    ITemplateClassBuilder CreateTemplateClassBuilder(Type viewBaseType, string className, params Type[] genericArguments);
    Type BuildView(CompilationContext compilationContext);
  }
}