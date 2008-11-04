using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

using NHaml.Utils;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public abstract class NHamlMvcView<TModel> : Template, IView, IViewDataContainer
    where TModel : class
  {
    private ViewContext _viewContext;

    private ViewDataDictionary<TModel> _viewData;

    public void Render(ViewContext viewContext, TextWriter writer)
    {
      Invariant.ArgumentNotNull(viewContext, "viewContext");

      _viewContext = viewContext;

      SetViewData(viewContext.ViewData);

      CreateHelpers(viewContext);

      Render(writer);
    }

    protected virtual void CreateHelpers(ViewContext viewContext)
    {
      Ajax = new AjaxHelper(viewContext);
      Html = new HtmlHelper(viewContext, this);
      Url = new UrlHelper(viewContext);
    }

    public AjaxHelper Ajax { get; protected set; }
    public HtmlHelper Html { get; protected set; }
    public UrlHelper Url { get; protected set; }

    public ViewContext ViewContext
    {
      get { return _viewContext; }
    }

    public ViewDataDictionary<TModel> ViewData
    {
      get { return _viewData; }
    }

    public TempDataDictionary TempData
    {
      get { return _viewContext.TempData; }
    }

    public TModel Model
    {
      get { return _viewData.Model; }
    }

    [SuppressMessage("Microsoft.Usage", "CA2227")]
    [SuppressMessage("Microsoft.Design", "CA1033")]
    ViewDataDictionary IViewDataContainer.ViewData
    {
      get { return _viewData; }
      set { SetViewData(value); }
    }

    private void SetViewData(ViewDataDictionary viewData)
    {
      if (typeof(ViewDataDictionary<TModel>).IsAssignableFrom(viewData.GetType()))
      {
        _viewData = (ViewDataDictionary<TModel>)viewData;
      }
      else
      {
        _viewData = new ViewDataDictionary<TModel>(viewData);
      }
    }
  }
}