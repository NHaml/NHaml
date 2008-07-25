using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;

using NHaml.Utilities;

namespace NHaml.Web.Mvc
{
  [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
  public abstract class MvcView<TModel> : IMvcView
    where TModel : class
  {
    private ViewContext _viewContext;
    private AjaxHelper _ajax;
    private HtmlHelper _html;
    private UrlHelper _url;

    private ViewDataDictionary<TModel> _viewData;

    public void Render(ViewContext viewContext)
    {
      viewContext.ArgumentNotNull("viewContext");

      _viewContext = viewContext;

      SetViewData(viewContext.ViewData);

      _ajax = new AjaxHelper(_viewContext);
      _html = new HtmlHelper(_viewContext, this);
      _url = new UrlHelper(_viewContext);

      ((ICompiledTemplate)this).Render(viewContext.HttpContext.Response.Output);
    }

    public AjaxHelper Ajax
    {
      get { return _ajax; }
    }

    public HtmlHelper Html
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