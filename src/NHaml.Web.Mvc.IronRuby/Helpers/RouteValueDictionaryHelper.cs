using System.Collections;
using System.Web.Routing;

namespace NHaml.Web.Mvc.IronRuby.Helpers
{
    public static class RouteValueDictionaryHelper
    {
        public static RouteValueDictionary ToRouteDictionary( this IDictionary dictionary )
        {
            var routeValueDictionary = new RouteValueDictionary();

            foreach( var key in dictionary.Keys )
            {
                routeValueDictionary.Add( key.ToString(), (dictionary[key] ?? string.Empty).ToString() );
            }

            return routeValueDictionary;
        }
    }
}