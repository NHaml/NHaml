using System;
using System.IO;

namespace NHaml
{
  public sealed class TemplateOutputWriter : IOutputWriter
  {
    private TextWriter _textWriter;

    private bool _indentComing = true;

    public TextWriter TextWriter
    {
      get { return _textWriter; }
      set { _textWriter = value; }
    }

    public void WriteLine(string value)
    {
      WriteIndent();

      _textWriter.WriteLine(value);

      _indentComing = true;
    }

    public void Write(string value)
    {
      WriteIndent();

      _textWriter.Write(value);
    }

    public void Indent()
    {
      Depth++;
    }

    public void Outdent()
    {
      Depth = Math.Max(0, Depth - 1);
    }

    public int Depth { get; set; }

    private void WriteIndent()
    {
      if (_indentComing)
      {
        _textWriter.Write(string.Empty.PadLeft(Depth * 2));

        _indentComing = false;
      }
    }
  }
}