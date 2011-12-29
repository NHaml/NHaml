using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Parser
{
    public class HamlNodeHtmlComment : HamlNode
    {
        private readonly string _commentText;

        public HamlNodeHtmlComment(IO.HamlLine nodeLine)
            : this (nodeLine.Content)
        { }

        public HamlNodeHtmlComment(string commentText)
        {
            _commentText = commentText;
        }

        public string CommentText
        {
            get { return _commentText; }
        }
    }
}
