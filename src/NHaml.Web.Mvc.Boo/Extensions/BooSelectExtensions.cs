using System.Collections.Generic;
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
    public static class BooSelectExtensions
    {

        public static ReturnString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, Hash htmlAttributes)
        {
            return htmlHelper.DropDownList( name, selectList, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }


        public static ReturnString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, Hash htmlAttributes)
        {
            return htmlHelper.DropDownList( name, selectList, optionLabel, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }



        public static ReturnString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, Hash htmlAttributes)
        {
            return htmlHelper.ListBox( name, selectList, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }



    }
}