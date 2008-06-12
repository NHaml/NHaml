using System;
using System.IO;

using NHaml.Properties;

namespace NHaml.Utilities
{
  internal static class Invariant
  {
    public static void ArgumentNotNull(object argument, string argumentName)
    {
      if (argument == null)
      {
        throw new ArgumentNullException(argumentName);
      }
    }

    public static void ArgumentNotEmpty(string argument, string argumentName)
    {
      if (argument == null)
      {
        throw new ArgumentNullException(argumentName);
      }

      if (argument.Length == 0)
      {
        throw new ArgumentOutOfRangeException(
          StringUtils.FormatCurrentCulture(Resources.StringCannotBeEmpty, argumentName));
      }
    }

    public static void FileExists(string path)
    {
      ArgumentNotEmpty(path, "path");

      if (!File.Exists(path))
      {
        throw new FileNotFoundException(path);
      }
    }
  }
}