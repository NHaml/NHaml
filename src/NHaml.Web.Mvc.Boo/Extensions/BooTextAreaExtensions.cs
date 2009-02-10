using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Boo.Lang;

using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
    public static class BooTextAreaExtensions
    {
        public static String TextArea( this HtmlHelper htmlHelper, String name, Hash htmlAttributes )
        {
            return TextAreaExtensions.TextArea( htmlHelper, name, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String TextArea( this HtmlHelper htmlHelper, String name, String value, Hash htmlAttributes )
        {
            return TextAreaExtensions.TextArea( htmlHelper, name, value, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String TextArea( this HtmlHelper htmlHelper, String name, String value, Int32 rows, Int32 columns, Hash htmlAttributes )
        {
            return TextAreaExtensions.TextArea( htmlHelper, name, value, rows, columns, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }
    }
}