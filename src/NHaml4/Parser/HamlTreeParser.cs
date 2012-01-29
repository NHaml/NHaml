using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.IO;
using System.IO;
using NHaml4.TemplateResolution;
using NHaml4.Parser.Rules;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Parser
{
    public class HamlTreeParser : ITreeParser
    {
        private readonly HamlFileLexer _hamlFileLexer;

        public HamlTreeParser(HamlFileLexer hamlFileLexer)
        {
            _hamlFileLexer = hamlFileLexer;
        }

        public HamlDocument ParseViewSource(IViewSource layoutViewSource)
        {
            HamlDocument result = null;
            using (var streamReader = layoutViewSource.GetStreamReader())
            {
                result = ParseStreamReader(streamReader);
            }
            return result;
        }

        public HamlDocument ParseDocumentSource(string documentSource)
        {
            using (var streamReader = new StreamReader(
                new MemoryStream(new UTF8Encoding().GetBytes(documentSource))))
            {
                return ParseStreamReader(streamReader);
            }
        }

        public HamlDocument ParseStreamReader(StreamReader reader)
        {
            var hamlFile = _hamlFileLexer.Read(reader);
            return ParseHamlFile(hamlFile);
        }

        public HamlDocument ParseHamlFile(HamlFile hamlFile)
        {
            var result = new HamlDocument();

            ParseNode(result, hamlFile);

            return result;
        }

        private void ParseNode(HamlNode node, HamlFile hamlFile)
        {
            node.IsMultiLine = true;
            while ((!hamlFile.EndOfFile) && (hamlFile.CurrentLine.IndentCount > node.IndentCount))
            {
                HamlLine nodeLine = hamlFile.CurrentLine;
                HamlNode childNode = GetHamlNode(nodeLine);
                node.AddChild(childNode);

                hamlFile.MoveNext();
                if (hamlFile.EndOfFile == false
                    && hamlFile.CurrentLine.IndentCount > nodeLine.IndentCount)
                {
                    childNode.AddChild(new HamlNodeText(new HamlLine("\n", nodeLine.SourceFileLineNo)));
                    ParseNode(childNode, hamlFile);
                }
                if (hamlFile.EndOfFile == false
                    && hamlFile.CurrentLine.IndentCount >= nodeLine.IndentCount)
                {
                    node.AddChild(new HamlNodeText(new HamlLine("\n", nodeLine.SourceFileLineNo)));
                }
            }
        }

        private HamlNode GetHamlNode(HamlLine nodeLine)
        {
            switch (nodeLine.HamlRule)
            {
                case HamlRuleEnum.PlainText:
                    return new HamlNodeText(nodeLine);
                case HamlRuleEnum.Tag:
                    return new HamlNodeTag(nodeLine);
                case HamlRuleEnum.HamlComment:
                    return new HamlNodeHamlComment(nodeLine);
                case HamlRuleEnum.HtmlComment:
                    return new HamlNodeHtmlComment(nodeLine);
                default:
                    throw new HamlUnknownRuleException(nodeLine.Content, nodeLine.SourceFileLineNo);
            }
        }
    }
}
