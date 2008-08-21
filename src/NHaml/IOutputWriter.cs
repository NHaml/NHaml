namespace NHaml
{
  public interface IOutputWriter
  {
    void WriteLine(string value);
    void Write(string value);
    void Indent();
    void Outdent();
  }
}