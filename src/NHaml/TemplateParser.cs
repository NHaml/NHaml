using System.Collections.Generic;
using NHaml.Compilers;
using NHaml.Rules;
using NHaml.TemplateResolution;

namespace NHaml
{
    public delegate void BlockClosingAction();

    public sealed class TemplateParser
    {
        private readonly string _singleIndent;

        public TemplateParser( 
            TemplateEngine templateEngine, TemplateClassBuilder templateClassBuilder,
            IList<IViewSource> layoutPaths, IViewSource templatePath2)
        {
            BlockClosingActions = new Stack<BlockClosingAction>();
            TemplateEngine = templateEngine;
            TemplateClassBuilder = templateClassBuilder;
            TemplatePath = templatePath2;
            MergedTemplatePaths = new List<IViewSource>();
            MergedTemplatePaths.AddRange(layoutPaths);
            MergedTemplatePaths.Add(templatePath2);

            Meta = new Dictionary<string, string>();

            if( TemplateEngine.Options.UseTabs )
            {
                _singleIndent = "\t";
            }
            else
            {
                _singleIndent = string.Empty.PadLeft( TemplateEngine.Options.IndentSize );
            }
        }

        public Dictionary<string, string> Meta { get; private set; }

        public TemplateEngine TemplateEngine { get; private set; }

    	public TemplateClassBuilder TemplateClassBuilder { get; private set; }

    	public IViewSource TemplatePath { get; private set; }

        public List<IViewSource> MergedTemplatePaths { get; private set; }

        private LinkedList<InputLine> InputLines { get; set; }

        private LinkedListNode<InputLine> CurrentNode { get; set; }

        private LinkedListNode<InputLine> NextNode
        {
            get { return CurrentNode.Next; }
        }

        public InputLine CurrentInputLine
        {
            get { return CurrentNode.Value; }
        }

        private InputLine NextInputLine
        {
            get { return CurrentNode.Next.Value; }
        }

        public Stack<BlockClosingAction> BlockClosingActions { get; private set; }

        public bool IsBlock
        {
            get { return NextInputLine.IndentCount > CurrentInputLine.IndentCount; }
        }

        public string NextIndent
        {
            get { return CurrentInputLine.Indent + _singleIndent; }
        }

        public int CurrentTemplateIndex { get; set; }

        public void Parse()
        {
            InputLines = new LinkedList<InputLine>();
            InputLines.AddLast( new InputLine( string.Empty, 0, TemplateEngine.Options.IndentSize ) );

            InputLines.AddLast( new InputLine( EofMarkupRule.SignifierChar, 1, TemplateEngine.Options.IndentSize ) );

            CurrentNode = InputLines.First.Next;
            for (CurrentTemplateIndex = 0; CurrentTemplateIndex < MergedTemplatePaths.Count; CurrentTemplateIndex++)
            {
                var templatePath = MergedTemplatePaths[CurrentTemplateIndex];
                MergeTemplate(templatePath, false);

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

                    var rule = TemplateEngine.GetRule(CurrentInputLine);
                    CurrentInputLine.NormalizedText = GetNormalizedText(rule, CurrentInputLine);
                    rule.Process(this);
                }

                CloseBlocks();
            }
        }


        public string GetNormalizedText(MarkupRule markupRule, InputLine inputLine)
        {
            var length = markupRule.Signifier.Length;
            var text = inputLine.Text;
            text = text.TrimStart();
            return text.Substring(length, text.Length - length);
        }

        public void MoveNext()
        {
            CurrentNode = CurrentNode.Next;
        }

        public void MergeTemplate( IViewSource templatePath, bool replaceCurrentNode )
        {

            var previous = CurrentNode.Previous;

            var lineNumber = 0;

            using( var reader = templatePath.GetStreamReader())
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    var inputLine = new InputLine(CurrentNode.Value.Indent + line, lineNumber++,
                                                  TemplateEngine.Options.IndentSize );
                    InputLines.AddBefore(CurrentNode, inputLine);
                }
            }
            if (replaceCurrentNode)
            {
                InputLines.Remove(CurrentNode);

            }
                CurrentNode = previous.Next;

        }

        public void CloseBlocks()
        {
            for( var j = 0;
                 ((j <= CurrentNode.Previous.Value.IndentCount - CurrentInputLine.IndentCount) && (BlockClosingActions.Count > 0));
                 j++ )
            {
                var blockClosingAction = BlockClosingActions.Pop();
                blockClosingAction();
            }
        }

    }
}