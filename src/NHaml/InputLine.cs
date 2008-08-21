using System.Text.RegularExpressions;

using NHaml.Exceptions;
using NHaml.Properties;

namespace NHaml
{
  public sealed class InputLine
  {
    private static readonly Regex _indentRegex
      = new Regex(@"^\s*", RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex _multiLineRegex
      = new Regex(@"^.+\s+(\|\s*)$", RegexOptions.Compiled | RegexOptions.Singleline);

    private string _text;
    private string _normalizedText;

    private readonly string _indent;
    private readonly char _signifier;
    private readonly int _lineNumber;
    private readonly int _indentSize;

    private bool _isMultiline;

    public InputLine(string text, int lineNumber)
    {
      _text = text;
      _lineNumber = lineNumber;

      var match = _multiLineRegex.Match(_text);

      _isMultiline = match.Success;

      if (_isMultiline)
      {
        _text = _text.Remove(match.Groups[1].Index);
      }

      _normalizedText = _text.Trim();

      if (!string.IsNullOrEmpty(_normalizedText))
      {
        _signifier = _normalizedText[0];
        _normalizedText = _normalizedText.Remove(0, 1);
      }

      _indent = _indentRegex.Match(_text).Groups[0].Value;

      ValidateIndentation();

      _indentSize = _indent.Length / 2;
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

    public int IndentSize
    {
      get { return _indentSize; }
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
      _normalizedText += ' ' + nextInputLine.Text.Trim();
      _isMultiline = nextInputLine.IsMultiline;
    }

    public override string ToString()
    {
      return LineNumber + ": " + Text;
    }

    private void ValidateIndentation()
    {
      if (_indent.Contains("\t") || ((_indent.Length % 2) != 0))
      {
        SyntaxException.Throw(this, Resources.IllegalIndentation);
      }
    }
  }
}