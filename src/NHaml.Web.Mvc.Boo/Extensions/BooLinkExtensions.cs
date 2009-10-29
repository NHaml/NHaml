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
    public static class BooLinkExtensions
    {

        public static ReturnString ActionLink( this HtmlHelper htmlHelper, string linkText, string actionName, Hash routeValues )
        {
            return htmlHelper.ActionLink( linkText, actionName, HashHelper.ToRouteValueDictionary( routeValues ) );
        }

        public static ReturnString ActionLink( this HtmlHelper htmlHelper, string linkText, string actionName, Hash routeValues, Hash htmlAttributes )
        {
            return htmlHelper.ActionLink( linkText, actionName, HashHelper.ToRouteValueDictionary( routeValues ), HashHelper.ToRouteValueDictionary( htmlAttributes ) );
        }

        public static ReturnString ActionLink( this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, Hash routeValues, Hash htmlAttributes )
        {
            return htmlHelper.ActionLink( linkText, actionName, controllerName, HashHelper.ToRouteValueDictionary( routeValues ), HashHelper.ToRouteValueDictionary( htmlAttributes ) );
        }

        public static ReturnString ActionLink( this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string protocol, string hostName, string fragment, Hash routeValues, Hash htmlAttributes )
        {
            return htmlHelper.ActionLink( linkText, actionName, controllerName, protocol, hostName, fragment, HashHelper.ToRouteValueDictionary( routeValues ), HashHelper.ToRouteValueDictionary( htmlAttributes ) );
        }

        public static ReturnString RouteLink( this HtmlHelper htmlHelper, string linkText, Hash routeValues )
        {
            return htmlHelper.RouteLink( linkText, HashHelper.ToRouteValueDictionary( routeValues ) );
        }

        public static ReturnString RouteLink( this HtmlHelper htmlHelper, string linkText, string routeName, Hash routeValues )
        {
            return htmlHelper.RouteLink( linkText, routeName, HashHelper.ToRouteValueDictionary( routeValues ) );
        }

        public static ReturnString RouteLink( this HtmlHelper htmlHelper, string linkText, Hash routeValues, Hash htmlAttributes )
        {
            return htmlHelper.RouteLink( linkText, HashHelper.ToRouteValueDictionary( routeValues ), HashHelper.ToRouteValueDictionary( htmlAttributes ) );
        }

        public static ReturnString RouteLink( this HtmlHelper htmlHelper, string linkText, string routeName, Hash routeValues, Hash htmlAttributes )
        {
            return htmlHelper.RouteLink( linkText, routeName, HashHelper.ToRouteValueDictionary( routeValues ), HashHelper.ToRouteValueDictionary( htmlAttributes ) );
        }

        public static ReturnString RouteLink( this HtmlHelper htmlHelper, string linkText, string routeName, string protocol, string hostName, string fragment, Hash routeValues, Hash htmlAttributes )
        {
            return htmlHelper.RouteLink( linkText, routeName, protocol, hostName, fragment, HashHelper.ToRouteValueDictionary( routeValues ), HashHelper.ToRouteValueDictionary( htmlAttributes ) );
        }

    }
}