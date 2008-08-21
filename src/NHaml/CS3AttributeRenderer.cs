using System.Diagnostics.CodeAnalysis;

namespace NHaml
{
  [SuppressMessage("Microsoft.Naming", "CA1722")]
  public sealed class CS3AttributeRenderer : IAttributeRenderer
  {
    public void Render(CompilationContext compilationContext, string attributes)
    {
      compilationContext.TemplateClassBuilder
        .AppendCode("Utility.RenderAttributes(new {" + attributes + "})");
    }
  }
}