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
            return FormExtensions.BeginForm( htmlHelper, HashHelper.ToRouteValueDictionary( values ) );
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         Hash values )
        {
            return FormExtensions.BeginForm( htmlHelper,
                                             actionName,
                                             controllerName,
                                             HashHelper.ToRouteValueDictionary( values ) );
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         Hash values,
                                         FormMethod method )
        {
            return FormExtensions.BeginForm( htmlHelper,
                                             actionName,
                                             controllerName,
                                             HashHelper.ToRouteValueDictionary( values ),
                                             method );
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         FormMethod method,
                                         Hash htmlAttributes )
        {
            return FormExtensions.BeginForm( htmlHelper,
                                             actionName,
                                             controllerName,
                                             method,
                                             HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static MvcForm BeginForm( this HtmlHelper htmlHelper,
                                         String actionName,
                                         String controllerName,
                                         Hash values,
                                         FormMethod method,
                                         Hash htmlAttributes )
        {
            return FormExtensions.BeginForm( htmlHelper,
                                             actionName,
                                             controllerName,
                                             HashHelper.ToRouteValueDictionary( values ),
                                             method,
                                             HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }
    }
}