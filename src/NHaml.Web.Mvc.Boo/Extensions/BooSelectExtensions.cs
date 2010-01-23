using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Boo.Lang;
using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
    public static class BooSelectExtensions
    {

        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, Hash htmlAttributes)
        {
            return htmlHelper.DropDownList( name, selectList, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }


        public static MvcHtmlString DropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, Hash htmlAttributes)
        {
            return htmlHelper.DropDownList( name, selectList, optionLabel, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }



        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, Hash htmlAttributes)
        {
            return htmlHelper.ListBox( name, selectList, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }



    }
}