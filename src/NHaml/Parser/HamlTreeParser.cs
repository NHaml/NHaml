using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml.Parser
{
    public class HamlTreeParser : ITreeParser
    {
        private readonly TemplateOptions _options;

        public HamlTreeParser()
        {
            //_options = options;
        }

        public HamlTree Parse(IList<TemplateResolution.IViewSource> layoutViewSources)
        {
            return new HamlTree();
            //viewSourceReader.DeQueueViewSource();
            //while (viewSourceReader.CurrentNode.Next != null)
            //{
            //    var rule = viewSourceReader.GetRule();

            //    // TODO - The Rule.Render method violates Command-Query Separation, is this fixable?

            //    if (rule.PerformCloseActions)
            //    {
            //        CloseBlocks(viewSourceReader);
            //        BlockClosingActions.Push(rule.Render(viewSourceReader, Options, builder));
            //    }
            //    else
            //    {
            //        rule.Render(viewSourceReader, Options, builder);
            //    }
            //    viewSourceReader.MoveNext();
            //}

            //CloseBlocks(viewSourceReader);
        }
    }
}
