using System;

namespace NHaml.Backends
{
  public interface ICompilerBackend
  {
    IAttributeRenderer AttributeRenderer { get; }
    ISilentEvalRenderer SilentEvalRenderer { get; }
    ITemplateClassBuilder CreateTemplateClassBuilder(Type viewBaseType, string className, params Type[] genericArguments);
    Type BuildView(CompilationContext compilationContext);
  }
}