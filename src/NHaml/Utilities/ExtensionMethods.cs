using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

using NHaml.Properties;

namespace NHaml.Utilities
{
  public static class ExtensionMethods
  {
    public static void IsNotNull(this object maybeNull)
    {
      if (maybeNull == null)
      {
        throw new InvalidOperationException(Resources.ObjectNull);
      }
    }

    public static void ArgumentNotNull(this object argument, string argumentName)
    {
      if (argument == null)
      {
        throw new ArgumentNullException(argumentName);
      }
    }

    public static void ArgumentNotEmpty(this string argument, string argumentName)
    {
      if (argument == null)
      {
        throw new ArgumentNullException(argumentName);
      }

      if (argument.Length == 0)
      {
        throw new ArgumentOutOfRangeException(
          Resources.StringCannotBeEmpty.FormatCurrentCulture(argumentName));
      }
    }

    public static void FileExists(this string path)
    {
      path.ArgumentNotEmpty("path");

      if (!File.Exists(path))
      {
        throw new FileNotFoundException(path);
      }
    }

    public static string RenderAttributes(this object attributeSource)
    {
      if (attributeSource != null)
      {
        var properties = TypeDescriptor.GetProperties(attributeSource);

        if (properties.Count > 0)
        {
          var attributes = new StringBuilder();

          AppendAttribute(attributeSource, properties[0], attributes, null);

          for (var i = 1; i < properties.Count; i++)
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