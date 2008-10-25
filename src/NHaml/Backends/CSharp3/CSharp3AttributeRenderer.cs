using System.Diagnostics.CodeAnalysis;

namespace NHaml.BackEnds.CSharp3
{
  [SuppressMessage("Microsoft.Naming", "CA1722")]
  public sealed class CSharp3AttributeRenderer : IAttributeRenderer
  {
    public void Render(CompilationContext compilationContext, string attributes)
    {
      compilationContext.TemplateClassBuilder
        .AppendCode("Utility.RenderAttributes(new {" + attributes + "})");
    }
  }
}