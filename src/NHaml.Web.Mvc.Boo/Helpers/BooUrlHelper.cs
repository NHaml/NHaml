using System;
using System.Web.Mvc;
using System.Web.Routing;

using Boo.Lang;

namespace NHaml.Web.Mvc.Boo.Helpers
{
    public class BooUrlHelper : UrlHelper
    {
        public BooUrlHelper( RequestContext requestContext )
            : base( requestContext )
        {
        }

        public BooUrlHelper( RequestContext requestContext, RouteCollection routeCollection )
            : base( requestContext, routeCollection )
        {
        }

        public String Action( String actionName, Hash valuesDictionary )
        {
            return Action( actionName, HashHelper.ToRouteValueDictionary( valuesDictionary ) );
        }

        public String Action( String actionName, String controllerName, Hash valuesDictionary )
        {
            return Action( actionName, controllerName, HashHelper.ToRouteValueDictionary( valuesDictionary ) );
        }

        public String Action( String actionName, String controllerName, Hash valuesDictionary, String protocol, String hostName )
        {
            return Action( actionName, controllerName, HashHelper.ToRouteValueDictionary( valuesDictionary ), protocol, hostName );
        }

        public String RouteUrl( Hash valuesDictionary )
        {
            return RouteUrl( HashHelper.ToRouteValueDictionary( valuesDictionary ) );
        }

        public String RouteUrl( String routeName, Hash valuesDictionary )
        {
            return RouteUrl( routeName, HashHelper.ToRouteValueDictionary( valuesDictionary ) );
        }

        public String RouteUrl( String routeName, Hash valuesDictionary, String protocol, String hostName )
        {
            return RouteUrl( routeName, HashHelper.ToRouteValueDictionary( valuesDictionary ), protocol, hostName );
        }
    }
}