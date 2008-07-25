using System.Collections.Generic;
using System.IO;

using NHaml.Rules;
using NHaml.Utilities;

namespace NHaml
{
  public sealed class CompilationContext
  {
    private readonly TemplateCompiler _templateCompiler;
    private readonly TemplateClassBuilder _templateClassBuilder;

    private readonly string _templatePath;
    private readonly string _layoutPath;

    private readonly LinkedList<InputLine> _inputLines
      = new LinkedList<InputLine>();

    private LinkedListNode<InputLine> _currentNode;

    private readonly Stack<BlockClosingAction> _blockClosingActions
      = new Stack<BlockClosingAction>();

    private readonly StringSet _inputFiles
      = new StringSet();

    public CompilationContext(TemplateCompiler templateCompiler, TemplateClassBuilder templateClassBuilder,
      string templatePath, string layoutPath)
    {
      _templateCompiler = templateCompiler;
      _templateClassBuilder = templateClassBuilder;
      _templatePath = templatePath;
      _layoutPath = layoutPath;

      var primaryTemplate = _layoutPath ?? _templatePath;

      _inputLines = BuildInputLines(primaryTemplate);

      _currentNode = _inputLines.First.Next;

      _inputFiles.Add(primaryTemplate);
    }

    public TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }

    public TemplateClassBuilder TemplateClassBuilder
    {
      get { return _templateClassBuilder; }
    }

    public string LayoutPath
    {
      get { return _layoutPath; }
    }

    public string TemplatePath
    {
      get { return _templatePath; }
    }

    public LinkedList<InputLine> InputLines
    {
      get { return _inputLines; }
    }

    public LinkedListNode<InputLine> CurrentNode
    {
      get { return _currentNode; }
    }

    public LinkedListNode<InputLine> NextNode
    {
      get { return _currentNode.Next; }
    }

    public InputLine CurrentInputLine
    {
      get { return _currentNode.Value; }
    }

    public InputLine NextInputLine
    {
      get { return _currentNode.Next.Value; }
    }

    public Stack<BlockClosingAction> BlockClosingActions
    {
      get { return _blockClosingActions; }
    }

    public void CollectInputFiles(ICollection<string> inputFiles)
    {
      inputFiles.Clear();

      foreach (var inputFile in _inputFiles)
      {
        inputFiles.Add(inputFile);
      }
    }

    public void MoveNext()
    {
      _currentNode = _currentNode.Next;
    }

    public void CloseBlocks()
    {
      for (var j = 0;
           ((j <= CurrentNode.Previous.Value.IndentSize
             - CurrentInputLine.IndentSize)
               && (_blockClosingActions.Count > 0));
           j++)
      {
        _blockClosingActions.Pop()();
      }
    }

    public void MergeTemplate(string templatePath)
    {
      var previous = _currentNode.Previous;

      var lineNumber = 0;

      using (var reader = new StreamReader(templatePath))
      {
        string line;

        while ((line = reader.ReadLine()) != null)
        {
          _inputLines.AddBefore(_currentNode,
            new InputLine(_currentNode.Value.Indent + line, lineNumber++));
        }
      }

      _inputLines.Remove(_currentNode);

      _currentNode = previous.Next;

      _inputFiles.Add(templatePath);
    }

    private LinkedList<InputLine> BuildInputLines(string templatePath)
    {
      var lineNumber = 0;

      _inputLines.AddLast(new InputLine(string.Empty, lineNumber++));

      using (var reader = new StreamReader(templatePath))
      {
        string line;

        while ((line = reader.ReadLine()) != null)
        {
          _inputLines.AddLast(new InputLine(line, lineNumber++));
        }
      }

      _inputLines.AddLast(new InputLine(EofMarkupRule.SignifierChar.ToString(), lineNumber));

      return _inputLines;
    }
  }
}