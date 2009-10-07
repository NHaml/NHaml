using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class FilterRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[]{":"};}
        }

        public override AstNode Process(ParserReader parserReader)
        {
            var reader = new CharacterReader(parserReader.Text,0);
            
            reader.Read(2); // eat :

            var name = reader.ReadName();

            var node = new FilterNode(name);

            node.Child = parserReader.ParseLines(parserReader.Indent, node.Child);
            
            return node;
        }
    }
}