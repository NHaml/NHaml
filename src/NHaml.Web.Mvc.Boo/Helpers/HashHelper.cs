using System;
using System.Collections.Generic;
using System.Web.Routing;

using Boo.Lang;

namespace NHaml.Web.Mvc.Boo.Helpers
{
    public static class HashHelper
    {
        public static RouteValueDictionary ToRouteValueDictionary( Hash hash )
        {
            if( hash == null )
                return null;

            var routeValues = new RouteValueDictionary();

            foreach( System.Collections.DictionaryEntry entry in hash )
            {
                routeValues.Add( Convert.ToString( entry.Key ), entry.Value );
            }

            return routeValues;
        }

        public static IDictionary<string, object> ToStringKeyDictinary( Hash hash )
        {
            if( hash == null )
                return null;

            var dictionary = new Dictionary<string, object>();

            foreach( System.Collections.DictionaryEntry entry in hash )
            {
                dictionary.Add( Convert.ToString( entry.Key ), entry.Value );
            }

            return dictionary;
        }
    }
}