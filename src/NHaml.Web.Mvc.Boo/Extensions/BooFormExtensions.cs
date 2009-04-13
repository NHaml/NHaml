using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Boo.Lang;

using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
    public static class BooFormExtensions
    {
        public static MvcForm BeginForm( this HtmlHelper htmlHelper, Hash values )
        {
            return htmlHelper.BeginForm(HashHelper.ToRouteValueDictionary( values ));
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         Hash values )
        {
            return htmlHelper.BeginForm(actionName, controllerName, HashHelper.ToRouteValueDictionary( values ));
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         Hash values,
                                         FormMethod method )
        {
            return htmlHelper.BeginForm(actionName, controllerName, HashHelper.ToRouteValueDictionary( values ), method);
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         FormMethod method,
                                         Hash htmlAttributes )
        {
            return htmlHelper.BeginForm(actionName, controllerName, method, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         Hash values,
                                         FormMethod method,
                                         Hash htmlAttributes )
        {
            return htmlHelper.BeginForm(actionName, controllerName, HashHelper.ToRouteValueDictionary( values ), method, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }
    }
}