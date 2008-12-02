using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Boo.Lang;

using NHaml.Web.Mvc.Boo.Helpers;

namespace NHaml.Web.Mvc.Boo.Extensions
{
  public static class BooSelectExtensions
  {
    public static String DropDownList( this HtmlHelper htmlHelper, String optionLabel, String name, Hash htmlAttributes )
    {
      return SelectExtensions.DropDownList( htmlHelper, optionLabel, name, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }

    public static String DropDownList( this HtmlHelper htmlHelper, String name, Hash htmlAttributes )
    {
      return SelectExtensions.DropDownList( htmlHelper, name, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }

    public static String DropDownList( this HtmlHelper htmlHelper, String name, SelectList selectList, Hash htmlAttributes )
    {
      return SelectExtensions.DropDownList( htmlHelper, name, selectList, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }

    public static String DropDownList( this HtmlHelper htmlHelper,
                                       String optionLabel,
                                       String name,
                                       SelectList selectList,
                                       Hash htmlAttributes )
    {
      return SelectExtensions.DropDownList( htmlHelper, optionLabel, name, selectList, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }

    public static String ListBox( this HtmlHelper htmlHelper, String name, Hash htmlAttributes )
    {
      return SelectExtensions.ListBox( htmlHelper, name, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }

    public static String ListBox( this HtmlHelper htmlHelper, String name, MultiSelectList selectList, Hash htmlAttributes )
    {
      return SelectExtensions.ListBox( htmlHelper, name, selectList, HashHelper.ToStringKeyDictinary( htmlAttributes ) );
    }
  }
}