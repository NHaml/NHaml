using System;

namespace NHaml.Core.Ast
{
    public class TextNode : AstNode
    {
        public TextNode(string text)
        {
            if(text == null)
                throw new ArgumentNullException("text");

            Text = text;
        }

        public string Text { get; set; }
        public bool IsInline { get; set; }
    }
}