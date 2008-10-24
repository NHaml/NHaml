using System;

using NHaml.Backends.CSharp2;
using NHaml.Exceptions;

namespace NHaml.Backends.CSharp3
{
  public class CSharp3CompilerBackend : ICompilerBackend
  {
    public CSharp3CompilerBackend()
    {
      AttributeRenderer = new CSharp3AttributeRenderer();
      LambdaRenderer = new CSharp3LambdaRenderer();
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

      var typeBuilder = new CSharp3TemplateTypeBuilder(compilationContext.TemplateCompiler);

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