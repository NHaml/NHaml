using System.Collections.Generic;
using NHaml.Rules;
using NHaml.TemplateResolution;

namespace NHaml
{
    public class ViewSourceReader 
    {
        private string _singleIndent;
        private LinkedList<InputLine> inputLines;
        private TemplateOptions options;

        public ViewSourceReader(TemplateOptions options, IList<IViewSource> viewSources)
        {
            this.options = options;
            ViewSources = viewSources;
            ViewSourceQueue = new Queue<IViewSource>();
            ViewSourceModifiedChecks = new List<Func<bool>>();

            //do a for here to avoid a modified closure
            for (var index = 0; index < viewSources.Count; index++)
            {
                var viewSource = viewSources[index];
                ViewSourceModifiedChecks.Add(() => viewSource.IsModified);
                ViewSourceQueue.Enqueue(viewSource);
            }


            if (options.UseTabs)
            {
                _singleIndent = "\t";
            }
            else
            {
                _singleIndent = string.Empty.PadLeft(options.IndentSize);
            }
            inputLines = new LinkedList<InputLine>();
            inputLines.AddLast(new InputLine(string.Empty, 0, options.IndentSize));
            inputLines.AddLast(new InputLine(EofMarkupRule.SignifierChar, 1, options.IndentSize));
            CurrentNode = inputLines.First.Next;
        }

        public IList<Func<bool>> ViewSourceModifiedChecks { get; private set; }

        public IList<IViewSource> ViewSources { get; set; }

        public Queue<IViewSource> ViewSourceQueue { get; set; }


        public LinkedListNode<InputLine> CurrentNode { get; private set; }


        public InputLine CurrentInputLine
        {
            get { return CurrentNode.Value; }
        }


        public InputLine NextInputLine
        {
            get { return CurrentNode.Next.Value; }
        }

        public bool IsBlock
        {
            get { return NextInputLine.IndentCount > CurrentInputLine.IndentCount; }
        }

        public string NextIndent
        {
            get { return CurrentInputLine.Indent + _singleIndent; }
        }


        public MarkupRule GetRule()
        {
            while (CurrentInputLine.IsMultiline && NextInputLine.IsMultiline)
            {
                CurrentInputLine.Merge(NextInputLine);
                inputLines.Remove(CurrentNode.Next);
            }

            if (CurrentInputLine.IsMultiline)
            {
                CurrentInputLine.TrimEnd();
            }

            CurrentInputLine.ValidateIndentation();

            var rule = options.GetRule(CurrentInputLine);
            CurrentInputLine.NormalizedText = GetNormalizedText(rule, CurrentInputLine);
            return rule;
        }


        private static string GetNormalizedText(MarkupRule markupRule, InputLine inputLine)
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

        public void MergeTemplate(IViewSource viewSource, bool replaceCurrentNode)
        {

            var previous = CurrentNode.Previous;

            var lineNumber = 0;

            using (var reader = viewSource.GetStreamReader())
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    //Swallow empty lines
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    var inputLine = new InputLine(CurrentNode.Value.Indent + line, lineNumber++, options.IndentSize);
                    inputLines.AddBefore(CurrentNode, inputLine);
                }
            }
            if (replaceCurrentNode)
            {
                inputLines.Remove(CurrentNode);
            }
            CurrentNode = previous.Next;

        }


    }
}