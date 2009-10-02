using System;
using NHaml.Core.Ast;

namespace NHaml.Core.Parser.Rules
{
    public abstract class MarkupRuleBase
    {
        public abstract string[] Signifiers { get; }

        public bool IsMatch(string text)
        {
            if(text == null)
                throw new ArgumentNullException("text");

            return GetMatchingSignifier(text) != null;
        }

        protected string GetMatchingSignifier(string text)
        {
            foreach(var signifier in Signifiers)
                if(text.StartsWith(signifier, StringComparison.InvariantCultureIgnoreCase))
                    return signifier;

            return null;
        }

        public abstract AstNode Process(ParserReader reader);

        public static bool IsNameChar(char ch)
        {
            return char.IsNumber(ch) ||
                   char.IsLetter(ch) ||
                   ch == '_' ||
                   ch == '-' ||
                   ch == ':';
        }
    }
}