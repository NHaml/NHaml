using System.Text.RegularExpressions;

using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml
{
  public sealed class InputLine
  {
    private static readonly Regex _indentRegex
      = new Regex(@"^(\s*|\t*)", RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _multiLineRegex
      = new Regex(@"^.+\s+(\|\s*)$", RegexOptions.Compiled | RegexOptions.Singleline);

    private string _text;
    private string _normalizedText;

    private readonly string _indent;
    private readonly char _signifier;
    private readonly int _lineNumber;
    private readonly int _indentSize;
    private readonly int _indentCount;

    private readonly bool _isMultiline;

    public InputLine(string text, int lineNumber)
      : this(text, lineNumber, 2)
    {
    }

    public InputLine(string text, int lineNumber, int indentSize)
    {
      _text = text;
      _lineNumber = lineNumber;
      _indentSize = indentSize;

      var match = _multiLineRegex.Match(_text);

      _isMultiline = match.Success;

      if (_isMultiline)
      {
        _text = _text.Remove(match.Groups[1].Index);
      }

      _normalizedText = _text.TrimStart();

      if (!string.IsNullOrEmpty(_normalizedText))
      {
        _signifier = _normalizedText[0];
        _normalizedText = _normalizedText.Remove(0, 1);
      }

      _indent = _indentRegex.Match(_text).Groups[0].Value;
      _indentCount = _indent.Length / _indentSize;
    }

    public char Signifier
    {
      get { return _signifier; }
    }

    public string Text
    {
      get { return _text; }
    }

    public string NormalizedText
    {
      get { return _normalizedText; }
    }

    public string Indent
    {
      get { return _indent; }
    }

    public int IndentCount
    {
      get { return _indentCount; }
    }

    public int LineNumber
    {
      get { return _lineNumber; }
    }

    public bool IsMultiline
    {
      get { return _isMultiline; }
    }

    public void Merge(InputLine nextInputLine)
    {
      _text += nextInputLine.Text.TrimStart();
      _normalizedText += nextInputLine.Text.TrimStart();
    }

    public void TrimEnd()
    {
      _text = _text.TrimEnd();
      _normalizedText = _normalizedText.TrimEnd();
    }

    public void ValidateIndentation()
    {
      if ((_indent.Length % _indentSize) != 0)
      {
        SyntaxException.Throw(this, Resources.IllegalIndentationSpaces, _indentSize);
      }
    }

    public override string ToString()
    {
      return LineNumber + ": " + Text;
    }
  }
}