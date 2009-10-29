using System.Web.Mvc;
using System.Web.Mvc.Html;
using IronRuby.Builtins;
#if NET4
using ReturnString = System.Web.Mvc.MvcHtmlString;
#else
using ReturnString = String;
#endif

namespace NHaml.Web.Mvc.IronRuby.Helpers
{
    public class NHamlMvcIronRubyHtmlHelper : HtmlHelper
    {
        // because IronRuby CLS method resolution is still flakey.

        public NHamlMvcIronRubyHtmlHelper( ViewContext context, IViewDataContainer viewDataContainer )
            : base( context, viewDataContainer )
        {
        }

        public ReturnString ActionLink(string linkText, Hash values)
        {
            return this.RouteLink( linkText, values.ToRouteDictionary() );
        }

        public ReturnString TextBox(string name, object value)
        {
            return InputExtensions.TextBox( this, name, value );
        }

        public ReturnString TextBox(string name)
        {
            return InputExtensions.TextBox( this, name );
        }

        public ReturnString DropDownList(string name, SelectList selectList)
        {
            return SelectExtensions.DropDownList( this, name, selectList );
        }

        public void RenderPartial(string partialViewName)
        {
            RenderPartialExtensions.RenderPartial( this, partialViewName );
        }
    }
}