using System;

using NHaml.Exceptions;

namespace NHaml.BackEnds.CSharp2
{
  public class CSharp2CompilerBackEnd : ICompilerBackEnd
  {
    public CSharp2CompilerBackEnd()
    {
      AttributeRenderer = new CSharp2AttributeRenderer();
      SilentEvalRenderer = new CSharp2SilentEvalRenderer(new CSharp2LambdaRenderer());
    }

    public IAttributeRenderer AttributeRenderer { get; protected set; }
    public ISilentEvalRenderer SilentEvalRenderer { get; protected set; }

    public ITemplateClassBuilder CreateTemplateClassBuilder(Type viewBaseType,
      string className, params Type[] genericArguments)
    {
      return new CSharp2TemplateClassBuilder(viewBaseType, className, genericArguments);
    }

    protected virtual CSharp2TemplateTypeBuilder CreateTemplateTypeBuilder(CompilationContext compilationContext)
    {
      return new CSharp2TemplateTypeBuilder(compilationContext.TemplateCompiler);
    }

    public Type BuildView(CompilationContext compilationContext)
    {
      var source = compilationContext.TemplateClassBuilder.Build();
      var typeBuilder = CreateTemplateTypeBuilder(compilationContext);
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