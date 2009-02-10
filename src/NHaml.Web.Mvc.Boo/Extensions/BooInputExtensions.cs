using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Boo.Lang;

using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
    public static class BooInputExtensions
    {
        public static String CheckBox( this HtmlHelper htmlHelper, String name, Hash htmlAttributes )
        {
            return InputExtensions.CheckBox( htmlHelper, name, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String CheckBox( this HtmlHelper htmlHelper, String name, Boolean isChecked, Hash htmlAttributes )
        {
            return InputExtensions.CheckBox( htmlHelper, name, isChecked, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String Hidden( this HtmlHelper htmlHelper, String name, Object value, Hash htmlAttributes )
        {
            return InputExtensions.Hidden( htmlHelper, name, value, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String Password( this HtmlHelper htmlHelper, String name, Object value, Hash htmlAttributes )
        {
            return InputExtensions.Password( htmlHelper, name, value, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String RadioButton( this HtmlHelper htmlHelper, String name, Object value, Hash htmlAttributes )
        {
            return InputExtensions.RadioButton( htmlHelper, name, value, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String RadioButton( this HtmlHelper htmlHelper, String name, Object value, Boolean isChecked, Hash htmlAttributes )
        {
            return InputExtensions.RadioButton( htmlHelper, name, value, isChecked, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }

        public static String TextBox( this HtmlHelper htmlHelper, String name, Object value, Hash htmlAttributes )
        {
            return InputExtensions.TextBox( htmlHelper, name, value, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
        }
    }
}