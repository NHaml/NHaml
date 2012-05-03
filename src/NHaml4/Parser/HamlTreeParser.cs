using System.Text;
using NHaml4.IO;
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

        public HamlDocument ParseViewSource(IViewSource layoutViewSource)
        {
            using (var streamReader = layoutViewSource.GetStreamReader())
            {
                return ParseStreamReader(streamReader, layoutViewSource.FileName);
            }
        }

        public HamlDocument ParseDocumentSource(string documentSource, string fileName)
        {
            using (var streamReader = new StreamReader(
                new MemoryStream(new UTF8Encoding().GetBytes(documentSource))))
            {
                return ParseStreamReader(streamReader, fileName);
            }
        }

        private HamlDocument ParseStreamReader(TextReader reader, string fileName)
        {
            var hamlFile = _hamlFileLexer.Read(reader, fileName);
            return ParseHamlFile(hamlFile);
        }

        public HamlDocument ParseHamlFile(HamlFile hamlFile)
        {
            var result = new HamlDocument(hamlFile.FileName);

            ParseNode(result, hamlFile);

            return result;
        }

        private void ParseNode(HamlNode node, HamlFile hamlFile)
        {
            node.IsMultiLine = true;
            while ((!hamlFile.EndOfFile) && (hamlFile.CurrentLine.IndentCount > node.IndentCount))
            {
                var nodeLine = hamlFile.CurrentLine;
                var childNode = HamlNodeFactory.GetHamlNode(nodeLine);
                node.AddChild(childNode);

                hamlFile.MoveNext();
                if (hamlFile.EndOfFile == false
                    && hamlFile.CurrentLine.IndentCount > nodeLine.IndentCount)
                {
                    childNode.AppendInnerTagNewLine();
                    ParseNode(childNode, hamlFile);
                }
                if (hamlFile.EndOfFile == false
                    && hamlFile.CurrentLine.IndentCount >= nodeLine.IndentCount)
                {
                    node.AppendPostTagNewLine(childNode, hamlFile.CurrentLine.SourceFileLineNo);
                }
            }
        }
    }
}
