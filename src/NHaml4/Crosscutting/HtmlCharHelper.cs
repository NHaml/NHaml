using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHaml4.Crosscutting
{
    public static class HtmlCharHelper
    {
        public static bool IsHtmlIdentifierChar(Char curChar)
        {
            return (Char.IsLetterOrDigit(curChar)
                    || curChar == '_'
                    || curChar == '-');
        }
    }
}
