using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Boo.Lang;
using NHaml.Web.Mvc.Boo.Helpers;
#if NET4
using ReturnString = System.Web.Mvc.MvcHtmlString;
#else
using ReturnString = String;
#endif

namespace NHaml.Web.Mvc.Boo.Extensions
{
    public static class BooTextAreaExtensions
    {
        public static ReturnString TextArea(this HtmlHelper htmlHelper, String name, Hash htmlAttributes)
        {
            return htmlHelper.TextArea(name, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }

        public static ReturnString TextArea(this HtmlHelper htmlHelper, String name, String value, Hash htmlAttributes)
        {
            return htmlHelper.TextArea(name, value, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }

        public static ReturnString TextArea(this HtmlHelper htmlHelper, String name, String value, Int32 rows, Int32 columns, Hash htmlAttributes)
        {
            return htmlHelper.TextArea(name, value, rows, columns, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }
    }
}