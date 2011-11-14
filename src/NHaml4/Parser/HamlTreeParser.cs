using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.IO;
using NHaml4.IO;

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

        public HamlDocument ParseDocument(IList<TemplateResolution.IViewSource> layoutViewSources)
        {
            var hamlFile = _hamlFileReader.Read(layoutViewSources[0].GetStreamReader());
            var result = new HamlDocument();
            while (hamlFile.CurrentLine != null)
            {
                result.AddChild(ParseNode(hamlFile));
                //hamlFile.MoveNext();
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

        private HamlNode ParseNode(HamlFile hamlFile)
        {
            // TODO - Node type?s
            HamlLine nodeLine = hamlFile.CurrentLine;
            HamlNode node = new HamlNode(nodeLine);
            hamlFile.MoveNext();
            while (hamlFile.CurrentLine != null && hamlFile.CurrentLine.IndentCount > nodeLine.IndentCount)
            {
                node.AddChild(ParseNode(hamlFile));
                hamlFile.MoveNext();
            }
            return node;
        }
    }
}
