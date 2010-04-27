using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Boo.Lang;
using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
    public static class BooValidationExtensions
    {
        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, String modelName, Hash htmlAttributes)
        {
            return htmlHelper.ValidationMessage(modelName, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }

        public static MvcHtmlString ValidationMessage(this HtmlHelper htmlHelper, String modelName, String validationMessage, Hash htmlAttributes)
        {
            return htmlHelper.ValidationMessage(modelName, validationMessage, HashHelper.ToStringKeyDictinary( htmlAttributes ));
        }

        public static MvcHtmlString ValidationSummary(this HtmlHelper htmlHelper, Hash htmlAttributes)
        {
            return htmlHelper.ValidationSummary("Validation errors occured.", HashHelper.ToStringKeyDictinary(htmlAttributes));
        }
    }
}