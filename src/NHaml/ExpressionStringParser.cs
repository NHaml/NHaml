using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NHaml
{
    public class ExpressionStringParser
    {
        private readonly string _expressionString;
        private static readonly Regex _parser;
        private static readonly Regex _escapedExpressionBeginQuotesRegex;
        private static readonly Regex _escapedExpressionEndQuotesRegex;

        public List<ExpressionStringToken> Tokens { get; protected set; }

        static ExpressionStringParser()
        {
            _parser = new Regex(@"#{(?<value>((?:\\\})|[^}])*)}", RegexOptions.Compiled);

            _escapedExpressionBeginQuotesRegex = new Regex( @"\\{", RegexOptions.Compiled );
            _escapedExpressionEndQuotesRegex = new Regex( @"\\}", RegexOptions.Compiled );
        }

        public ExpressionStringParser(string expressionString)
        {
            _expressionString = expressionString;
            Tokens = new List<ExpressionStringToken>(); 
        }

        public void Parse()
        {
            Tokens.Clear();

            var matches = _parser.Matches(_expressionString);

            var index = 0;
            foreach (Match match in matches)
            {
                if( index != match.Index )
                {
                    var token = _expressionString.Substring(index, match.Index - index);
                    Tokens.Add(new ExpressionStringToken(token, false));
                }

                var expressionToken = match.Groups["value"].Value;
                expressionToken = _escapedExpressionBeginQuotesRegex.Replace( expressionToken, "{" );
                expressionToken = _escapedExpressionEndQuotesRegex.Replace( expressionToken, "}" );

                Tokens.Add( new ExpressionStringToken( expressionToken, true ) );

                index = match.Index + match.Length;
            }

            if( index != _expressionString.Length )
            {
                var token = _expressionString.Substring( index, _expressionString.Length-index );
                Tokens.Add( new ExpressionStringToken( token, false ) );
            }

            if( Tokens.Count == 0 )
                Tokens.Add(new ExpressionStringToken(string.Empty, false));
        }
    }
}