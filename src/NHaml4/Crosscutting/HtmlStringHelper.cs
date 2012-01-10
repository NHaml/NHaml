using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHaml4.Parser.Exceptions;

namespace NHaml4.Crosscutting
{
    public static class HtmlStringHelper
    {
        enum AttributeParseStates { Normal, SingleQuoteEscaped, DoubleQuoteEscaped };

        public static bool IsHtmlIdentifierChar(Char curChar)
        {
            return (Char.IsLetterOrDigit(curChar)
                    || curChar == '_'
                    || curChar == '-');
        }

        public static string ExtractTokenFromTagString(string inputString, ref int index, char[] endMarkers)
        {
            AttributeParseStates state = AttributeParseStates.Normal;
            int startIndex = index;

            for (; index < inputString.Length; index++)
            {
                switch (state)
                {
                    case AttributeParseStates.Normal:
                        if (inputString[index] == '\'')
                            state = AttributeParseStates.SingleQuoteEscaped;
                        else if (inputString[index] == '\"')
                            state = AttributeParseStates.DoubleQuoteEscaped;
                        else if (endMarkers.Contains(inputString[index]))
                            return inputString.Substring(startIndex, index - startIndex + 1);
                        break;
                    case AttributeParseStates.SingleQuoteEscaped:
                        if (inputString[index] == '\'')
                            state = AttributeParseStates.Normal;
                        break;
                    case AttributeParseStates.DoubleQuoteEscaped:
                        if (inputString[index] == '\"')
                            state = AttributeParseStates.Normal;
                        break;
                }
            }

            return inputString;
        }
    }
}
