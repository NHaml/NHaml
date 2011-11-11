using System.Collections.Generic;
using NHaml.Compilers;
using NHaml.TemplateResolution;

namespace NHaml
{
    public delegate void BlockClosingAction();

    public class TemplateParser 
    {
        public TemplateParser(TemplateOptions options)
        {
            BlockClosingActions = new Stack<BlockClosingAction>();
            Options = options;
        }

        public TemplateOptions Options { get; private set; }

        public Stack<BlockClosingAction> BlockClosingActions { get; private set; }

        public void Parse(ViewSourceReader viewSourceReader, TemplateClassBuilder builder)
        {
            viewSourceReader.DeQueueViewSource();
            while (viewSourceReader.CurrentNode.Next != null)
            {
                var rule = viewSourceReader.GetRule();

                // TODO - The Rule.Render method violates Command-Query Separation, is this fixable?

                if (rule.PerformCloseActions)
                {
                    CloseBlocks(viewSourceReader);
                    BlockClosingActions.Push(rule.Render(viewSourceReader, Options, builder));
                }
                else
                {
                    rule.Render(viewSourceReader, Options, builder);
                }
                viewSourceReader.MoveNext();
            }

            CloseBlocks(viewSourceReader);
        }

        public void CloseBlocks(ViewSourceReader viewSourceReader)
        {
            var currentIndentCount = viewSourceReader.CurrentInputLine.IndentCount;
            var previousIndentCount = viewSourceReader.CurrentNode.Previous.Value.IndentCount;
            for (var index = 0; (index <= previousIndentCount - currentIndentCount) && (BlockClosingActions.Count > 0); index++)
            {
                var blockClosingAction = BlockClosingActions.Pop();
                blockClosingAction();
            }
        }
    }
}