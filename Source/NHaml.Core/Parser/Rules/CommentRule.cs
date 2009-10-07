using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class CommentRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[] {"/", "-#"}; }
        }

        public override AstNode Process(ParserReader parser)
        {
            if(parser.Text.StartsWith("-#"))
            {
                // skip all children
                parser.ParseChildren(parser.Indent, null);
                return null;
            }

            var node = new CommentNode();
            var reader = new CharacterReader(parser.Text,0);
            
            reader.Read(2); // eat /

            ReadConditionIfExists(reader, node);

            reader.ReadWhiteSpaces();

            if(!reader.IsEndOfStream)
                node.Child = parser.ParseText(reader.ReadName(),reader.Index);

            node.Child = parser.ParseChildren(parser.Indent, node.Child);

            return node;
        }

        private static void ReadConditionIfExists(CharacterReader reader, CommentNode node)
        {
            if(reader.Current != '[')
                return;
            
            reader.Read(); // eat [

            node.Condition = reader.ReadWhile(c => c != ']');
        }
    }
}