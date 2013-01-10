using System.Text;
using System.IO;
using System.Web.NHaml.IO;
using System.Web.NHaml.TemplateResolution;

namespace System.Web.NHaml.Parser
{
    public class HamlTreeParser : ITreeParser
    {
        private readonly HamlFileLexer _hamlFileLexer;

        public HamlTreeParser(HamlFileLexer hamlFileLexer)
        {
            _hamlFileLexer = hamlFileLexer;
        }

        public HamlDocument ParseViewSource(ViewSource layoutViewSource)
        {
            using (var streamReader = layoutViewSource.GetTextReader())
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
            //node.IsMultiLine = true;
            while ((!hamlFile.EndOfFile) && (hamlFile.CurrentLine.IndentCount > node.IndentCount))
            {
                var nodeLine = hamlFile.CurrentLine;
                var childNode = HamlNodeFactory.GetHamlNode(nodeLine);
                node.AddChild(childNode);

                hamlFile.MoveNext();
                if (hamlFile.EndOfFile == false
                    && hamlFile.CurrentLine.IndentCount > nodeLine.IndentCount)
                {
                    if (hamlFile.CurrentLine.IsInline == false) childNode.AppendInnerTagNewLine();
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
