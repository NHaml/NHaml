using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Web.Mvc;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public class NHamlHtmlHelper : HtmlHelper
  {
    private readonly IOutputWriter _outputWriter;

    private static readonly Regex _controllerSuffix = new Regex("Controller$",
      RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

    public NHamlHtmlHelper(IOutputWriter outputWriter, ViewContext viewContext, IViewDataContainer viewDataContainer)
      : base(viewContext, viewDataContainer)
    {
      _outputWriter = outputWriter;
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
    public virtual void Form<TModel>(TModel model, Action<FormBuilder<TModel>> yield)
      where TModel : IModel
    {
      var formBuilder = new FormBuilder<TModel>(model, this, _outputWriter);

      formBuilder.OpenTag(FormMethod.Post, new RouteValueDictionary());
      yield(formBuilder);
      formBuilder.CloseTag();
    }

    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public virtual string ActionLink<TController>()
      where TController : Controller
    {
      var controllerName = GetControllerName(typeof(TController));

      return ActionLink(controllerName, "index", controllerName.ToLowerInvariant());
    }

    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public virtual string ActionLink<TController>(RestfulAction restfulAction, string linkText)
      where TController : Controller
    {
      var controllerName = GetControllerName(typeof(TController));

      return ActionLink(linkText, restfulAction.ToString().ToLowerInvariant(), controllerName.ToLowerInvariant());
    }

    public virtual string ActionLink<TModel>(TModel model)
      where TModel : IModel
    {
      return ActionLink(model, RestfulAction.Show);
    }

    public virtual string ActionLink<TModel>(TModel model, RestfulAction restfulAction)
      where TModel : IModel
    {
      return ActionLink(model, restfulAction, model.ToString());
    }

    public virtual string ActionLink<TModel>(TModel model, RestfulAction restfulAction, string linkText)
      where TModel : IModel
    {
      // TODO: Delete should POST through a hidden form
      //       Should be able to specify "confirm" alert

      return ActionLink(
        linkText,
        restfulAction.ToString().ToLowerInvariant(),
        Inflector.Pluralize(model.GetType().Name).ToLowerInvariant(),
        new RouteValueDictionary {{"id", model.Id}},
        new RouteValueDictionary());
    }

    private static string GetControllerName(Type controllerType)
    {
      return _controllerSuffix.Replace(controllerType.Name, string.Empty);
    }
  }
}