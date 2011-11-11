using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NHaml.Utils
{
    public static class Utility
    {

        public static string MakeClassName( string templatePath )
        {
            Invariant.ArgumentNotEmpty( templatePath, "templatePath" );
            var stringBuilder = new StringBuilder();
            foreach (var ch in templatePath)
            {
                if ((ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90) || (ch >= 0 && ch <= 9))
                {
                    stringBuilder.Append(ch);
                }
                else
                {
                    stringBuilder.Append('_');
                }
            }
            return stringBuilder.ToString().Replace(templatePath, "_").Trim('_');
        }

        public static string MakeBaseClassName( Type baseType, string open, string close, string separator )
        {
            var typeName = baseType.FullName.Replace( "+", separator ).Replace( ".", separator );

            if( baseType.IsGenericType )
            {
                typeName = typeName.Substring( 0, typeName.IndexOf( '`' ) ) + open;

                var parameters = new List<string>();

                foreach( var t in baseType.GetGenericArguments() )
                {
                    parameters.Add( MakeBaseClassName( t, open, close, separator ) );
                }

                typeName += string.Join( ",", parameters.ToArray() ) + close;
            }

            return typeName;
        }

        public static string FormatCurrentCulture( string format, params object[] values )
        {
            return String.Format( CultureInfo.CurrentCulture, format, values );
        }

        public static string FormatInvariant( string format, params object[] values )
        {
            return String.Format( CultureInfo.InvariantCulture, format, values );
        }
    }
}