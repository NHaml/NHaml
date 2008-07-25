using System.IO;

namespace NHaml
{
  public interface ICompiledTemplate
  {
    void Render(TextWriter textWriter);
  }
}