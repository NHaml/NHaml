using System;
using System.IO;

using NHaml.Properties;

namespace NHaml.Utilities
{
  internal static class Invariant
  {
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
  }
}