using System.Web.Mvc;
using System.Web.Mvc.Html;

using IronRuby.Builtins;

namespace NHaml.Web.Mvc.IronRuby.Helpers
{
  public class NHamlMvcIronRubyHtmlHelper : HtmlHelper
  {
    // because IronRuby CLS method resolution is still flakey.

    public NHamlMvcIronRubyHtmlHelper(ViewContext context, IViewDataContainer viewDataContainer)
      : base(context, viewDataContainer)
    {
    }

    public string ActionLink(string linkText, Hash values)
    {
      return this.RouteLink(linkText, values.ToRouteDictionary());
    }

    public string TextBox(string name, object value)
    {
      return InputExtensions.TextBox(this, name, value);
    }

    public string TextBox(string name)
    {
      return InputExtensions.TextBox(this, name);
    }

    public string DropDownList(string name, SelectList selectList)
    {
      return SelectExtensions.DropDownList(this, name, selectList);
    }
  }
}