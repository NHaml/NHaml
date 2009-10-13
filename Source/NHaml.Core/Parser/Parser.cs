using System.IO;
using NHaml.Core.Ast;
using NHaml.Core.Parser.Rules;

namespace NHaml.Core.Parser
{
    public class Parser
    {
        private readonly MarkupRuleBase[] _rules;

        public Parser()
        {
            _rules = new MarkupRuleBase[]
            {
                new DocTypeMarkupRule(),
                new TagMarkupRule(),
                new CommentRule(),
                new FilterRule()
            };
        }

        public DocumentNode Parse(string input)
        {
            using(var reader = new StringReader(input))
            {
                return Parse(new InputReader(reader));
            }
        }

        public DocumentNode ParseFile(string fileName)
        {
            return ParseFile(new FileInfo(fileName));
        }

        public DocumentNode ParseFile(FileInfo file)
        {
            using(var reader = new StreamReader(file.FullName))
            {
                return Parse(new InputReader(reader));
            }
        }

        internal DocumentNode Parse(InputReader reader)
        {
            var parserReader = new ParserReader(_rules, reader);
            var document = new DocumentNode();

            while(parserReader.Read())
            {
                var node = parserReader.ParseNode();
                if(node != null)
                    document.Childs.Add(node);
            }

            return document;
        }
    }
}