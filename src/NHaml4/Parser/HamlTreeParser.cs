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
        private readonly HamlFileLexer _hamlFileReader;

        public HamlTreeParser(HamlFileLexer hamlFileReader)
        {
            _hamlFileReader = hamlFileReader;
        }

        public HamlDocument ParseDocument(IList<TemplateResolution.IViewSource> layoutViewSources)
        {
            var hamlFile = _hamlFileReader.Read(layoutViewSources[0].GetStreamReader());
            var result = new HamlDocument();
            while (!hamlFile.EndOfFile)
            {
                result.AddChild(ParseNode(hamlFile));
            }
            return result;
        }

        private HamlNode ParseNode(HamlFile hamlFile)
        {
            HamlLine nodeLine = hamlFile.CurrentLine;
            HamlNode node = new HamlNode(nodeLine);
            hamlFile.MoveNext();
            while (hamlFile.CurrentLine != null && hamlFile.CurrentLine.IndentCount > nodeLine.IndentCount)
            {
                node.AddChild(ParseNode(hamlFile));
            }
            return node;
        }
    }
}
