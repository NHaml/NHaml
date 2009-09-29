using System;
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
                new TagMarkupRule()
            };
        }

        public DocumentNode Parse(string input)
        {
            using(var reader = new StringReader(input))
            {
                return Parse(new InputLineReader(reader));
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
                return Parse(new InputLineReader(reader));
            }
        }

        internal DocumentNode Parse(InputLineReader reader)
        {
            var parserReader = new ParserReader(_rules, reader);
            var document = new DocumentNode();

            while(parserReader.Read())
                document.Chields.Add(parserReader.CreateNode());

            return document;
        }
    }
}