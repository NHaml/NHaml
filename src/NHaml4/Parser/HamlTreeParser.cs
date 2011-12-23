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
        private readonly HamlFileLexer _hamlFileLexer;

        public HamlTreeParser(HamlFileLexer hamlFileLexer)
        {
            _hamlFileLexer = hamlFileLexer;
        }

        public HamlDocument ParseViewSources(ViewSourceList layoutViewSources)
        {
            return ParseStreamReader(layoutViewSources[0].GetStreamReader());
        }

        public HamlDocument ParseDocumentSource(string documentSource)
        {
            var streamReader = new StreamReader(
                new MemoryStream(new UTF8Encoding().GetBytes(documentSource)));
            return ParseStreamReader(streamReader);
        }

        public HamlDocument ParseStreamReader(StreamReader reader)
        {
            var hamlFile = _hamlFileLexer.Read(reader);
            return ParseHamlFile(hamlFile);
        }

        public HamlDocument ParseHamlFile(HamlFile hamlFile)
        {
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
                    throw new HamlUnknownRuleException(nodeLine.Content);
            }
        }
    }
}
