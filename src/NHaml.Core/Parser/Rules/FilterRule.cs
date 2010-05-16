using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public class FilterRule : MarkupRuleBase
    {
        public override string[] Signifiers
        {
            get { return new[]{":"};}
        }

        public override AstNode Process(ParserReader parser)
        {
            var reader = parser.Input;
            
            reader.Skip(":");

            var name = reader.ReadName();

            var node = new FilterNode(name);

            node.Child = parser.ParseLines(parser.Indent, node.Child);
            
            return node;
        }
    }
}