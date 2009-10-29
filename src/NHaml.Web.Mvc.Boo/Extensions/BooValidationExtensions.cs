using System;
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
    public static class BooValidationExtensions
    {
        public static ReturnString ValidationMessage(this HtmlHelper htmlHelper, String modelName, Hash htmlAttributes)
        {
            return htmlHelper.ValidationMessage(modelName, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }

        public static ReturnString ValidationMessage(this HtmlHelper htmlHelper, String modelName, String validationMessage, Hash htmlAttributes)
        {
            return htmlHelper.ValidationMessage(modelName, validationMessage, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }

        public static ReturnString ValidationSummary(this HtmlHelper htmlHelper, Hash htmlAttributes)
        {
            return htmlHelper.ValidationSummary("Validation errors occured.", HashHelper.ToStringKeyDictinary(htmlAttributes));
        }
    }
}