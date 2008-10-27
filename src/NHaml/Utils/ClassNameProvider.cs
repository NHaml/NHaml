using System;
using System.Text.RegularExpressions;

namespace NHaml.Utils
{
  public static class ClassNameProvider
  {
    /*
     *     private static readonly Regex _pathCleaner
      = new Regex( @"[-\\/\.:\s]", RegexOptions.Compiled | RegexOptions.Singleline );
*/
    private static readonly Regex _pathCleaner
      = new Regex( @"[^0-9a-z/_]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline );

    public static string MakeClassName( string templatePath )
    {
      if( templatePath == null )
      {
        throw new ArgumentNullException( "templatePath" );
      }

      return _pathCleaner.Replace( templatePath, "_" ).Trim( '_' );
    }
  }
}
