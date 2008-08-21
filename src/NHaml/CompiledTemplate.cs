using System.IO;

namespace NHaml
{
  public abstract class CompiledTemplate
  {
    public virtual void Render(TextWriter textWriter)
    {
    }

    protected abstract TemplateOutputWriter Output { get; }
  }
}