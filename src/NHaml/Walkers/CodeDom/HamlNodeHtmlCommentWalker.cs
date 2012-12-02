using NHaml.Parser;
using NHaml.Compilers;
using NHaml.Parser.Rules;
namespace NHaml.Walkers.CodeDom
{
    public sealed class HamlNodeHtmlCommentWalker : HamlNodeWalker
    {
        public HamlNodeHtmlCommentWalker(ITemplateClassBuilder classBuilder, HamlHtmlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var commentNode = node as HamlNodeHtmlComment;
            if (commentNode == null)
                throw new System.InvalidCastException("HamlNodeHtmlCommentWalker requires that HamlNode object be of type HamlNodeHtmlComment.");

            ClassBuilder.Append(node.Indent);
            ClassBuilder.Append("<!--" + commentNode.Content);
       
            base.Walk(node);

            if (node.IsMultiLine)
            {
                ClassBuilder.AppendNewLine();
                ClassBuilder.Append(node.Indent + "-->");
            }
            else
            {
                ClassBuilder.Append(" -->");
            }

        }
    }
}
