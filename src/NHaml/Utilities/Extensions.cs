using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace NHaml.Utilities
{
  public static class Extensions
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

          var value = Convert.ToString(properties[0].GetValue(obj), CultureInfo.InvariantCulture);

          if (!string.IsNullOrEmpty(value))
          {
            attributes.Append(properties[0].Name.Replace('_', '-') + "=\"" + value + "\"");
          }

          for (var i = 1; i < properties.Count; i++)
          {
            value = Convert.ToString(properties[i].GetValue(obj), CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(value))
            {
              attributes.Append(" " + properties[i].Name.Replace('_', '-') + "=\"" + value + "\"");
            }
          }

          return attributes.ToString();
        }
      }

      return null;
    }
  }
}