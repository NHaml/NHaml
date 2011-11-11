using System.Web.Mvc;
using System.Web.Mvc.Html;
using IronRuby.Builtins;

namespace NHaml.Web.Mvc.IronRuby.Helpers
{
    public class NHamlMvcIronRubyHtmlHelper<TModel> : HtmlHelper<TModel>
    {
        // because IronRuby CLS method resolution is still flakey.

        public NHamlMvcIronRubyHtmlHelper( ViewContext context, IViewDataContainer viewDataContainer )
            : base( context, viewDataContainer )
        {
        }

        public MvcHtmlString ActionLink(string linkText, Hash values)
        {
            return this.RouteLink( linkText, values.ToRouteDictionary() );
        }

        public MvcHtmlString TextBox(string name, object value)
        {
            return InputExtensions.TextBox( this, name, value );
        }

        public MvcHtmlString TextBox(string name)
        {
            return InputExtensions.TextBox( this, name );
        }

        public MvcHtmlString DropDownList(string name, SelectList selectList)
        {
            return SelectExtensions.DropDownList( this, name, selectList );
        }

        public void RenderPartial(string partialViewName)
        {
            RenderPartialExtensions.RenderPartial( this, partialViewName );
        }
    }
}