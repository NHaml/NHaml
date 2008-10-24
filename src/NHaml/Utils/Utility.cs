using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace NHaml.Utils
{
  public static class Utility
  {
    public static string RenderAttributes(object attributeSource)
    {
      if (attributeSource != null)
      {
        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(attributeSource);

        if (properties.Count > 0)
        {
          var attributes = new StringBuilder();

          AppendAttribute(attributeSource, properties[0], attributes, null);

          for (int i = 1; i < properties.Count; i++)
          {
            AppendAttribute(attributeSource, properties[i], attributes, " ");
          }

          return attributes.ToString();
        }
      }

      return null;
    }

    private static void AppendAttribute(object obj, PropertyDescriptor propertyDescriptor,
      StringBuilder attributes, string separator)
    {
      object value = propertyDescriptor.GetValue(obj);
      string name = propertyDescriptor.Name.Replace('_', '-');

      AppendAttribute(value, attributes, separator, name);
    }

    private static void AppendAttribute(object value, StringBuilder attributes,
      string separator, object name)
    {
      string invariantValue = Convert.ToString(value, CultureInfo.InvariantCulture);
      string invariantName = Convert.ToString(name, CultureInfo.InvariantCulture);

      if (!string.IsNullOrEmpty(invariantValue))
      {
        attributes.Append(separator + invariantName + "=\"" + invariantValue + "\"");
      }
    }

    public static string FormatCurrentCulture(string format, params object[] values)
    {
      return String.Format(CultureInfo.CurrentCulture, format, values);
    }

    public static string FormatInvariant(string format, params object[] values)
    {
      return String.Format(CultureInfo.InvariantCulture, format, values);
    }
  }
}