using System.Web.Mvc;
using System.Web.Routing;

using IronRuby.Builtins;

namespace NHaml.Web.Mvc.IronRuby.Helpers
{
    public class NHamlMvcIronRubyUrlHelper : UrlHelper
    {
        // because IronRuby CLS method resolution is still flakey.

        public NHamlMvcIronRubyUrlHelper( RequestContext context )
            : base( context )
        {
        }

        public string Action( Hash values )
        {
            return RouteUrl( values.ToRouteDictionary() );
        }
    }
}