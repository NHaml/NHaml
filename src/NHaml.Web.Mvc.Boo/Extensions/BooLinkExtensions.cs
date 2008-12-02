using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Boo.Lang;

using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
  public static class BooLinkExtensions
  {
    public static String ActionLink( this HtmlHelper htmlHelper, String linkText, String actionName, Hash values )
    {
      return LinkExtensions.ActionLink( htmlHelper, linkText, actionName, HashHelper.ToRouteValueDictionary( values ) );
    }

    public static String ActionLink( this HtmlHelper htmlHelper, String linkText, String actionName, Hash values, Hash htmlAttributes )
    {
      return LinkExtensions.ActionLink( htmlHelper,
                                        linkText,
                                        actionName,
                                        HashHelper.ToRouteValueDictionary( values ),
                                        HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }

    public static String ActionLink( this HtmlHelper htmlHelper,
                                     String linkText,
                                     String actionName,
                                     String controllerName,
                                     Hash values,
                                     Hash htmlAttributes )
    {
      return LinkExtensions.ActionLink( htmlHelper,
                                        linkText,
                                        actionName,
                                        controllerName,
                                        HashHelper.ToRouteValueDictionary( values ),
                                        HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }

    public static String ActionLink( this HtmlHelper htmlHelper,
                                     String linkText,
                                     String actionName,
                                     String controllerName,
                                     String protocol,
                                     String hostName,
                                     String fragment,
                                     Hash values,
                                     Hash htmlAttributes )
    {
      return LinkExtensions.ActionLink( htmlHelper,
                                        linkText,
                                        actionName,
                                        controllerName,
                                        protocol,
                                        hostName,
                                        fragment,
                                        HashHelper.ToRouteValueDictionary( values ),
                                        HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }

    public static String GenerateLink( this HtmlHelper htmlHelper,
                                       String linkText,
                                       String routeName,
                                       String actionName,
                                       String controllerName,
                                       Hash values,
                                       Hash htmlAttributes )
    {
      return LinkExtensions.GenerateLink( htmlHelper,
                                          linkText,
                                          routeName,
                                          actionName,
                                          controllerName,
                                          HashHelper.ToRouteValueDictionary( values ),
                                          HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }

    public static String GenerateLink( this HtmlHelper htmlHelper,
                                       String linkText,
                                       String routeName,
                                       String actionName,
                                       String controllerName,
                                       String protocol,
                                       String hostName,
                                       String fragment,
                                       Hash values,
                                       Hash htmlAttributes )
    {
      return LinkExtensions.GenerateLink( htmlHelper,
                                          linkText,
                                          routeName,
                                          actionName,
                                          controllerName,
                                          protocol,
                                          hostName,
                                          fragment,
                                          HashHelper.ToRouteValueDictionary( values ),
                                          HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }

    public static String RouteLink( this HtmlHelper htmlHelper, String linkText, Hash values )
    {
      return LinkExtensions.RouteLink( htmlHelper, linkText, HashHelper.ToRouteValueDictionary( values ) );
    }

    public static String RouteLink( this HtmlHelper htmlHelper, String linkText, String name, Hash values )
    {
      return LinkExtensions.RouteLink( htmlHelper, linkText, name, HashHelper.ToRouteValueDictionary( values ) );
    }

    public static String RouteLink( this HtmlHelper htmlHelper, String linkText, Hash values, Hash htmlAttributes )
    {
      return LinkExtensions.RouteLink( htmlHelper,
                                       linkText,
                                       HashHelper.ToRouteValueDictionary( values ),
                                       HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }

    public static String RouteLink( this HtmlHelper htmlHelper, String linkText, String routeName, Hash values, Hash htmlAttributes )
    {
      return LinkExtensions.RouteLink( htmlHelper,
                                       linkText,
                                       routeName,
                                       HashHelper.ToRouteValueDictionary( values ),
                                       HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }

    public static String RouteLink( this HtmlHelper htmlHelper,
                                    String linkText,
                                    String routeName,
                                    String protocol,
                                    String hostName,
                                    String fragment,
                                    Hash values,
                                    Hash htmlAttributes )
    {
      return LinkExtensions.RouteLink( htmlHelper,
                                       linkText,
                                       routeName,
                                       protocol,
                                       hostName,
                                       fragment,
                                       HashHelper.ToRouteValueDictionary( values ),
                                       HashHelper.ToRouteValueDictionary( htmlAttributes ) );
    }
  }
}