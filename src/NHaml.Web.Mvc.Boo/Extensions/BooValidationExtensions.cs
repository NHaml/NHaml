using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Boo.Lang;

using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
  public static class BooValidationExtensions
  {
    public static String ValidationMessage( this HtmlHelper htmlHelper, String modelName, Hash htmlAttributes )
    {
      return ValidationExtensions.ValidationMessage( htmlHelper, modelName, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }

    public static String ValidationMessage( this HtmlHelper htmlHelper, String modelName, String validationMessage, Hash htmlAttributes )
    {
      return ValidationExtensions.ValidationMessage( htmlHelper,
                                                     modelName,
                                                     validationMessage,
                                                     HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }

    public static String ValidationSummary( this HtmlHelper htmlHelper, Hash htmlAttributes )
    {
      return ValidationExtensions.ValidationSummary( htmlHelper, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }
  }
}