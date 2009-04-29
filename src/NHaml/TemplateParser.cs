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
        private readonly string _singleIndent;

        public TemplateParser( 
            TemplateEngine templateEngine, TemplateClassBuilder templateClassBuilder,
            string templatePath, string layoutTemplatePath )
        {
            BlockClosingActions = new Stack<BlockClosingAction>();
            InputFiles = new StringSet();
            TemplateEngine = templateEngine;
            TemplateClassBuilder = templateClassBuilder;
            TemplatePath = templatePath;
            LayoutTemplatePath = layoutTemplatePath;

            string primaryTemplate;
            if( LayoutTemplatePath == null )
            {
                primaryTemplate = TemplatePath;
            }
            else
            {
                primaryTemplate = LayoutTemplatePath;
            }

            Meta = new Dictionary<string, string>();

            InputLines = BuildInputLines( primaryTemplate, templateEngine );

            CurrentNode = InputLines.First.Next;

            InputFiles.Add( primaryTemplate );

            _singleIndent = TemplateEngine.UseTabs ? "\t" : string.Empty.PadLeft( TemplateEngine.IndentSize );
        }

        public Dictionary<string, string> Meta { get; private set; }

        public TemplateEngine TemplateEngine { get; private set; }

        public TemplateClassBuilder TemplateClassBuilder { get; private set; }

        public string TemplatePath { get; private set; }

        public string LayoutTemplatePath { get; private set; }

        public StringSet InputFiles { get; private set; }

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

        public Stack<BlockClosingAction> BlockClosingActions { get; private set; }

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
            while( CurrentNode.Next != null )
            {
                while( CurrentInputLine.IsMultiline && NextInputLine.IsMultiline )
                {
                    CurrentInputLine.Merge( NextInputLine );
                    InputLines.Remove( NextNode );
                }

                if( CurrentInputLine.IsMultiline )
                {
                    CurrentInputLine.TrimEnd();
                }

                CurrentInputLine.ValidateIndentation();

                TemplateEngine.GetRule( CurrentInputLine ).Process( this );
            }

            CloseBlocks();
        }

        public void MoveNext()
        {
            CurrentNode = CurrentNode.Next;
        }

        public void MergeTemplate( string templatePath )
        {
            var previous = CurrentNode.Previous;

            var lineNumber = 0;

            using( var reader = new StreamReader( templatePath ) )
            {
                string line;

                while( (line = reader.ReadLine()) != null )
                {
                    InputLines.AddBefore( CurrentNode,
                      new InputLine( CurrentNode.Value.Indent + line, templatePath, lineNumber++, TemplateEngine.IndentSize ) );
                }
            }

            InputLines.Remove( CurrentNode );

            CurrentNode = previous.Next;

            InputFiles.Add( templatePath );
        }

        public void CloseBlocks()
        {
            for( var j = 0;
                 ((j <= CurrentNode.Previous.Value.IndentCount
                   - CurrentInputLine.IndentCount)
                     && (BlockClosingActions.Count > 0));
                 j++ )
            {
                BlockClosingActions.Pop()();
            }
        }

        private static LinkedList<InputLine> BuildInputLines( string templatePath, TemplateEngine templateEngine )
        {
            var lineNumber = 0;
            var inputLines = new LinkedList<InputLine>();
            inputLines.AddLast( new InputLine( string.Empty, null, lineNumber++, templateEngine.IndentSize ) );

            using( var reader = new StreamReader( templatePath ) )
            {
                string line;

                while( (line = reader.ReadLine()) != null )
                {
                    inputLines.AddLast( new InputLine( line, templatePath, lineNumber++, templateEngine.IndentSize ) );
                }
            }

            inputLines.AddLast( new InputLine( EofMarkupRule.SignifierChar.ToString(), null, lineNumber, templateEngine.IndentSize ) );

            return inputLines;
        }
    }
}