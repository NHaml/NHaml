using System;
using System.IO;

using NHaml.Properties;

namespace NHaml.Utils
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class Invariant
    {
        public static void IsNotNull( object maybeNull )
        {
            if( maybeNull == null )
            {
                throw new InvalidOperationException( Resources.ObjectNull );
            }
        }

        public static void ArgumentNotNull( object argument, string argumentName )
        {
            if( argument == null )
            {
                throw new ArgumentNullException( argumentName );
            }
        }

        public static void ArgumentNotEmpty( string argument, string argumentName )
        {
            if( argument == null )
            {
                throw new ArgumentNullException( argumentName );
            }

            if( argument.Length == 0 )
            {
                throw new ArgumentOutOfRangeException(
                  Utility.FormatCurrentCulture( Resources.StringCannotBeEmpty, argumentName ) );
            }
        }

        public static void FileExists( string path )
        {
            ArgumentNotEmpty( path, "path" );

            if( !File.Exists( path ) )
            {
                throw new FileNotFoundException( path );
            }
        }
    }
}