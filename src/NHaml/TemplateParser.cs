using System.Collections.Generic;
using System.IO;

using NHaml.Compilers;
using NHaml.Rules;
using NHaml.Utils;

namespace NHaml
{
  public delegate void BlockClosingAction();

  public sealed class TemplateParser
  {
    private readonly TemplateEngine _templateEngine;
    private readonly TemplateClassBuilder _templateClassBuilder;

    private readonly string _templatePath;
    private readonly string _layoutTemplatePath;
    private readonly string _singleIndent;

      private readonly Stack<BlockClosingAction> _blockClosingActions
      = new Stack<BlockClosingAction>();

    private readonly StringSet _inputFiles = new StringSet();

    public TemplateParser(TemplateEngine templateEngine, TemplateClassBuilder templateClassBuilder,
      string templatePath, string layoutTemplatePath)
    {
      _templateEngine = templateEngine;
      _templateClassBuilder = templateClassBuilder;
      _templatePath = templatePath;
      _layoutTemplatePath = layoutTemplatePath;

      string primaryTemplate;
        if (_layoutTemplatePath == null)
        {
            primaryTemplate = _templatePath;
        }
        else
        {
            primaryTemplate = _layoutTemplatePath;
        }

        InputLines = BuildInputLines(primaryTemplate, templateEngine);

      CurrentNode = InputLines.First.Next;

      _inputFiles.Add(primaryTemplate);

      _singleIndent = _templateEngine.UseTabs ? "\t" : string.Empty.PadLeft(_templateEngine.IndentSize);
    }

    public TemplateEngine TemplateEngine
    {
      get { return _templateEngine; }
    }

    public TemplateClassBuilder TemplateClassBuilder
    {
      get { return _templateClassBuilder; }
    }

    public string TemplatePath
    {
      get { return _templatePath; }
    }

    public string LayoutTemplatePath
    {
      get { return _layoutTemplatePath; }
    }

    public IEnumerable<string> InputFiles
    {
      get { return _inputFiles; }
    }

      public LinkedList<InputLine> InputLines { get; private set; }

      public LinkedListNode<InputLine> CurrentNode { get; private set; }

      public LinkedListNode<InputLine> NextNode
    {
      get { return CurrentNode.Next; }
    }

    public InputLine CurrentInputLine
    {
      get { return CurrentNode.Value; }
    }

    public InputLine NextInputLine
    {
      get { return CurrentNode.Next.Value; }
    }

    public Stack<BlockClosingAction> BlockClosingActions
    {
      get { return _blockClosingActions; }
    }

    public bool IsBlock
    {
      get { return NextInputLine.IndentCount > CurrentInputLine.IndentCount; }
    }

    public string NextIndent
    {
      get { return CurrentInputLine.Indent + _singleIndent; }
    }

    public void Parse()
    {
      while (CurrentNode.Next != null)
      {
        while (CurrentInputLine.IsMultiline && NextInputLine.IsMultiline)
        {
          CurrentInputLine.Merge(NextInputLine);
          InputLines.Remove(NextNode);
        }

        if (CurrentInputLine.IsMultiline)
        {
          CurrentInputLine.TrimEnd();
        }

        CurrentInputLine.ValidateIndentation();

        TemplateEngine.GetRule(CurrentInputLine).Process(this);
      }

      CloseBlocks();
    }

    public void MoveNext()
    {
      CurrentNode = CurrentNode.Next;
    }

    public void MergeTemplate(string templatePath)
    {
      var previous = CurrentNode.Previous;

      var lineNumber = 0;

      using (var reader = new StreamReader(templatePath))
      {
        string line;

        while ((line = reader.ReadLine()) != null)
        {
          InputLines.AddBefore(CurrentNode,
            new InputLine(CurrentNode.Value.Indent + line, templatePath, lineNumber++, _templateEngine.IndentSize));
        }
      }

      InputLines.Remove(CurrentNode);

      CurrentNode = previous.Next;

      _inputFiles.Add(templatePath);
    }

    public void CloseBlocks()
    {
      for (var j = 0;
           ((j <= CurrentNode.Previous.Value.IndentCount
             - CurrentInputLine.IndentCount)
               && (_blockClosingActions.Count > 0));
           j++)
      {
        _blockClosingActions.Pop()();
      }
    }

    private static LinkedList<InputLine> BuildInputLines(string templatePath, TemplateEngine templateEngine)
    {
      var lineNumber = 0;
        var inputLines = new LinkedList<InputLine>();
        inputLines.AddLast(new InputLine(string.Empty, null, lineNumber++, templateEngine.IndentSize));

      using (var reader = new StreamReader(templatePath))
      {
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            inputLines.AddLast(new InputLine(line, templatePath, lineNumber++, templateEngine.IndentSize));
        }
      }

      inputLines.AddLast(new InputLine(EofMarkupRule.SignifierChar.ToString(), null, lineNumber, templateEngine.IndentSize));

      return inputLines;
    }
  }
}