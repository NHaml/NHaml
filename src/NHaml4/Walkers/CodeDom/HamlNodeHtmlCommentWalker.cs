using NHaml4.Parser;
using NHaml4.Compilers;
using NHaml4.Crosscutting;
namespace NHaml4.Walkers.CodeDom
{
    public class HamlNodeHtmlCommentWalker : HamlNodeWalker, INodeWalker
    {
        public HamlNodeHtmlCommentWalker(ITemplateClassBuilder classBuilder, HamlOptions options)
            : base(classBuilder, options)
        { }

        public override void Walk(HamlNode node)
        {
            var commentNode = node as HamlNodeHtmlComment;
            if (commentNode == null)
                throw new System.InvalidCastException("HamlNodeHtmlCommentWalker requires that HamlNode object be of type HamlNodeHtmlComment.");

            _classBuilder.Append("<!--" + commentNode.CommentText);
            _classBuilder.Append(" -->");
        }
    }
}
