using System;
using System.Linq.Expressions;
using System.Web.Mvc;

using Microsoft.Web.Mvc.Internal;

namespace NHaml.Samples.Mvc
{
  public static class ControllerExtensions
  {
    public static RedirectToRouteResult RedirectToAction<T>(this T controller,
      Expression<Action<T>> action) where T : Controller
    {
      return ((Controller)controller).RedirectToAction(action);
    }

    public static RedirectToRouteResult RedirectToAction<T>(this Controller controller,
      Expression<Action<T>> action) where T : Controller
    {
      return new RedirectToRouteResult(ExpressionHelper.GetRouteValuesFromExpression(action));
    }
  }
}