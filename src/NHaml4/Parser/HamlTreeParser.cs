using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.IO;

namespace NHaml.Parser
{
    public class HamlTreeParser : ITreeParser
    {
        private readonly TemplateOptions _options;
        private readonly HamlFileReader _hamlFileReader;

        public HamlTreeParser(HamlFileReader hamlFileReader)
        {
            _hamlFileReader = hamlFileReader;
        }

        public HamlTree Parse(IList<TemplateResolution.IViewSource> layoutViewSources)
        {
            var result = new HamlTree();
            var hamlFile = _hamlFileReader.Read(layoutViewSources[0].GetStreamReader());
            while (hamlFile.CurrentLine != null)
            {
                result.AddChild(hamlFile.CurrentLine);
                hamlFile.MoveNext();
            }

            return result;
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
