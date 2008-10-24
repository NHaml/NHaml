using System;

using NHaml.Exceptions;

namespace NHaml.Backends.Boo
{
  public class BooCompilerBackend : ICompilerBackend
  {
    public BooCompilerBackend()
    {
      AttributeRenderer = new BooAttributeRenderer();
      SilentEvalRenderer = new BooSilentEvalRenderer();
    }

    #region ICompilerBackend Members

    public IAttributeRenderer AttributeRenderer { get; private set; }
    public ISilentEvalRenderer SilentEvalRenderer { get; private set; }

    public ITemplateClassBuilder CreateTemplateClassBuilder(
      Type viewBaseType,
      string className,
      params Type[] genericArguments)
    {
      return new BooTemplateClassBuilder(viewBaseType, className, genericArguments);
    }

    public Type BuildView(CompilationContext compilationContext)
    {
      var source = compilationContext.TemplateClassBuilder.Build();

      var typeBuilder = new BooTemplateTypeBuilder(compilationContext.TemplateCompiler);

      var viewType = typeBuilder.Build(source, compilationContext.TemplateClassBuilder.ClassName);

      if (viewType == null)
      {
        ViewCompilationException.Throw(typeBuilder.CompilerResults,
          typeBuilder.Source, compilationContext.TemplatePath);
      }

      return viewType;
    }

    #endregion
  }
}