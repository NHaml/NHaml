using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace NHaml.Utilities
{
  public static class ExtensionMethods
  {
    [SuppressMessage("Microsoft.Naming", "CA1720")]
    public static string RenderAttributes(this object obj)
    {
      if (obj != null)
      {
        var properties = TypeDescriptor.GetProperties(obj);

        if (properties.Count > 0)
        {
          var attributes = new StringBuilder();

          AppendAttribute(obj, properties[0], attributes, null);

          for (var i = 1; i < properties.Count; i++)
          {
            AppendAttribute(obj, properties[i], attributes, " ");
          }

          return attributes.ToString();
        }
      }

      return null;
    }

    private static void AppendAttribute(object obj, PropertyDescriptor propertyDescriptor,
      StringBuilder attributes, string separator)
    {
      var value = Convert.ToString(propertyDescriptor.GetValue(obj), CultureInfo.InvariantCulture);

      if (!string.IsNullOrEmpty(value))
      {
        attributes.Append(separator + propertyDescriptor.Name.Replace('_', '-') + "=\"" + value + "\"");
      }
    }

    public static string FormatCurrentCulture(this string format, params object[] values)
    {
      return String.Format(CultureInfo.CurrentCulture, format, values);
    }

    public static string FormatInvariant(this string format, params object[] values)
    {
      return String.Format(CultureInfo.InvariantCulture, format, values);
    }
  }
}