using System.Collections.Generic;
using NHaml.Compilers;
using NHaml.TemplateResolution;

namespace NHaml
{
    public delegate void BlockClosingAction();

    public class TemplateParser 
    {
        private TemplateClassBuilder builder;
        private ViewSourceReader viewSourceReader;
        public TemplateParser(TemplateOptions options, TemplateClassBuilder templateClassBuilder, IList<IViewSource> viewSources)
        {
            BlockClosingActions = new Stack<BlockClosingAction>();

            viewSourceReader = new ViewSourceReader(options, viewSources);
            Options = options;
            builder = templateClassBuilder;
        }


        public TemplateOptions Options { get; private set; }

        public Stack<BlockClosingAction> BlockClosingActions { get; private set; }

      
        public ViewSourceReader Parse()
        {
            ProcessViewSource(viewSourceReader.ViewSourceQueue.Dequeue());
            return viewSourceReader;
        }

        private void ProcessViewSource(IViewSource viewSource)
        {
            viewSourceReader.MergeTemplate(viewSource, false);

            while (viewSourceReader.CurrentNode.Next != null)
            {
                var rule = viewSourceReader.GetRule();
                if (rule.PerformCloseActions)
                {
                    CloseBlocks();
                    BlockClosingActions.Push(rule.Render(viewSourceReader, Options, builder));
                    viewSourceReader.MoveNext();
                }
                else
                {
                    rule.Render(viewSourceReader, Options, builder);
                }
            }

            CloseBlocks();
        }



        public void CloseBlocks()
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