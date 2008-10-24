using System;

using NHaml.Exceptions;

namespace NHaml.Backends.CSharp2
{
  public class CSharp2CompilerBackend : ICompilerBackend
  {
    public CSharp2CompilerBackend()
    {
      AttributeRenderer = new CSharp2AttributeRenderer();
      LambdaRenderer = new CSharp2LambdaRenderer();
      SilentEvalRenderer = new CSharp2SilentEvalRenderer(LambdaRenderer);
    }

    public ILambdaRenderer LambdaRenderer { get; private set; }

    public IAttributeRenderer AttributeRenderer { get; private set; }
    public ISilentEvalRenderer SilentEvalRenderer { get; private set; }

    public ITemplateClassBuilder CreateTemplateClassBuilder(
      Type viewBaseType,
      string className,
      params Type[] genericArguments)
    {
      return new CSharp2TemplateClassBuilder(viewBaseType, className, genericArguments);
    }

    public Type BuildView(CompilationContext compilationContext)
    {
      var source = compilationContext.TemplateClassBuilder.Build();

      var typeBuilder = new CSharp2TemplateTypeBuilder(compilationContext.TemplateCompiler);

      var viewType = typeBuilder.Build(source, compilationContext.TemplateClassBuilder.ClassName);

      if (viewType == null)
      {
        ViewCompilationException.Throw(typeBuilder.CompilerResults,
          typeBuilder.Source, compilationContext.TemplatePath);
      }

      return viewType;
    }
  }
}