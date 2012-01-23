using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
using NHaml4.Parser.Rules;

namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeTextWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeTextWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var nodeText = node as HamlNodeText;
            if (nodeText == null)
                throw new System.InvalidCastException("HamlNodeTextWalker requires that HamlNode object be of type HamlNodeText.");

            if ((node.Content.Trim().Length > 0)
                || (IsWhiteSpaceTrimmedDueToSurroundingNode(node) == false))
            {
                _classBuilder.Append(nodeText.Indent);
                _classBuilder.Append(nodeText.Content);
            }
            base.Walk(node);
        }

        private bool IsWhiteSpaceTrimmedDueToSurroundingNode(HamlNode node)
        {
            var previousNode = node.Previous;
            while (IsWhitespaceTextTag(previousNode))
                previousNode = previousNode.Previous;
            if (IsHamlNodeTagWithSurroundingWhitespaceRemoval(previousNode))
                return true;

            var nextNode = node.Next;
            while (IsWhitespaceTextTag(nextNode))
                nextNode = nextNode.Next;
            if (IsHamlNodeTagWithSurroundingWhitespaceRemoval(nextNode))
                return true;

            return false;
        }

        private bool IsWhitespaceTextTag(HamlNode node)
        {
            return node != null
                && node is HamlNodeText
                && ((HamlNodeText)node).IsWhitespace();
        }

        private bool IsHamlNodeTagWithSurroundingWhitespaceRemoval(HamlNode node)
        {
            return node != null
                && node is HamlNodeTag
                && ((HamlNodeTag)node).WhitespaceRemoval == WhitespaceRemoval.Surrounding;
        }

    }
}
