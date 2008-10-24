using System.Collections.Generic;
using System.IO;

using NHaml.Backends;
using NHaml.Rules;
using NHaml.Utils;

namespace NHaml
{
  public sealed class CompilationContext
  {
    private readonly IAttributeRenderer _attributeRenderer;

    private readonly Stack<BlockClosingAction> _blockClosingActions
      = new Stack<BlockClosingAction>();

    private readonly StringSet _inputFiles
      = new StringSet();

    private readonly LinkedList<InputLine> _inputLines
      = new LinkedList<InputLine>();

    private readonly string _layoutPath;

    private readonly ISilentEvalRenderer _silentEvalRenderer;
    private readonly ITemplateClassBuilder _templateClassBuilder;
    private readonly TemplateCompiler _templateCompiler;

    private readonly string _templatePath;

    private LinkedListNode<InputLine> _currentNode;

    public CompilationContext(TemplateCompiler templateCompiler,
      IAttributeRenderer attributeRenderer,
      ISilentEvalRenderer silentEvalRenderer,
      ITemplateClassBuilder templateClassBuilder,
      string templatePath, string layoutPath)
    {
      _templateCompiler = templateCompiler;
      _attributeRenderer = attributeRenderer;
      _silentEvalRenderer = silentEvalRenderer;
      _templateClassBuilder = templateClassBuilder;
      _templatePath = templatePath;
      _layoutPath = layoutPath;

      string primaryTemplate = _layoutPath ?? _templatePath;

      _inputLines = BuildInputLines(primaryTemplate);

      _currentNode = _inputLines.First.Next;

      _inputFiles.Add(primaryTemplate);
    }

    public TemplateCompiler TemplateCompiler
    {
      get { return _templateCompiler; }
    }

    public IAttributeRenderer AttributeRenderer
    {
      get { return _attributeRenderer; }
    }

    public ISilentEvalRenderer SilentEvalRenderer
    {
      get { return _silentEvalRenderer; }
    }

    public ITemplateClassBuilder TemplateClassBuilder
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

    public bool IsBlock
    {
      get
      {
        return NextInputLine.IndentSize >
          CurrentInputLine.IndentSize;
      }
    }

    public void CollectInputFiles(ICollection<string> inputFiles)
    {
      inputFiles.Clear();

      foreach (string inputFile in _inputFiles)
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
      for (int j = 0;
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
      LinkedListNode<InputLine> previous = _currentNode.Previous;

      int lineNumber = 0;

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
      int lineNumber = 0;

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