using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml.IO;
using NHaml4.IO;
using NHaml;
using System.IO;
using NHaml4.TemplateResolution;

namespace NHaml4.Parser
{
    public class HamlTreeParser : ITreeParser
    {
        private readonly TemplateOptions _options;
        private readonly HamlFileLexer _hamlFileReader;

        public HamlTreeParser(HamlFileLexer hamlFileReader)
        {
            _hamlFileReader = hamlFileReader;
        }

        public HamlDocument ParseDocument(IList<IViewSource> layoutViewSources)
        {
            return ParseDocument(layoutViewSources[0].GetStreamReader());
        }

        public HamlDocument ParseDocument(string documentSource)
        {
            var streamReader = new StreamReader(
                new MemoryStream(new System.Text.UTF8Encoding().GetBytes(documentSource)));
            return ParseDocument(streamReader);
        }

        private HamlDocument ParseDocument(StreamReader reader)
        {
            var hamlFile = _hamlFileReader.Read(reader);
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
            HamlNode node = GetHamlNode(nodeLine);

            hamlFile.MoveNext();
            while (hamlFile.CurrentLine != null && hamlFile.CurrentLine.IndentCount > nodeLine.IndentCount)
            {
                node.AddChild(ParseNode(hamlFile));
            }
            return node;
        }

        private HamlNode GetHamlNode(HamlLine nodeLine)
        {
            switch (nodeLine.HamlRule)
            {
                case HamlRuleEnum.PlainText:
                    return new HamlNodeText(nodeLine);
                default:
                    throw new HamlUnknownRuleException(nodeLine.HamlRule.ToString());
            }
        }
    }
}
