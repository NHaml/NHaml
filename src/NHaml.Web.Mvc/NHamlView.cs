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
  public abstract class NHamlView<TModel> : CompiledTemplate, INHamlView
    where TModel : class
  {
    private ViewContext _viewContext;
    private AjaxHelper _ajax;
    private NHamlHtmlHelper _html;
    private UrlHelper _url;

    private ViewDataDictionary<TModel> _viewData;

    public void Render(ViewContext viewContext, TextWriter writer)
    {
      Invariant.ArgumentNotNull(viewContext, "viewContext");

      _viewContext = viewContext;

      SetViewData(viewContext.ViewData);

      _ajax = new AjaxHelper(_viewContext);
      _html = new NHamlHtmlHelper(Output, _viewContext, this);
      _url = new UrlHelper(_viewContext);

      Render(viewContext.HttpContext.Response.Output);
    }

    public void Render(ViewContext viewContext)
    {
    }

    public AjaxHelper Ajax
    {
      get { return _ajax; }
    }

    public NHamlHtmlHelper Html
    {
      get { return _html; }
    }

    public UrlHelper Url
    {
      get { return _url; }
    }

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
        _viewData = (ViewDataDictionary<TModel>) viewData;
      }
      else
      {
        _viewData = new ViewDataDictionary<TModel>(viewData);
      }
    }
  }
}