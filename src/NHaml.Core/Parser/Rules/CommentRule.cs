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
            var reader = parser.Input;

            if(parser.Text.StartsWith("-#"))
            {
                // skip all children
                parser.ParseChildren(parser.Indent, null);
                return null;
            }

            var node = new CommentNode();

            reader.Skip("/");

            reader.SkipWhiteSpaces();

            if(reader.CurrentChar == '[')
            {
                reader.Skip("[");

                node.Condition = reader.ReadWhile(c => c != ']');
            }
            else if(!reader.IsEndOfStream)
            {
                var index = reader.Index;
                var text = reader.ReadToEnd();
                node.Child = parser.ParseText(text, index);
            }

            node.Child = parser.ParseChildren(parser.Indent, node.Child);

            return node;
        }
    }
}