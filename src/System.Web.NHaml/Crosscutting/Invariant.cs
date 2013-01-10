namespace System.Web.NHaml.Crosscutting
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class Invariant
    {
        public static void ArgumentNotNull( object argument, string argumentName )
        {
            if( argument == null )
                throw new ArgumentNullException( argumentName );
        }

        public static void ArgumentNotEmpty( string argument, string argumentName )
        {
            if( argument == null )
                throw new ArgumentNullException( argumentName );

            if( argument.Length == 0 )
                throw new ArgumentOutOfRangeException(
                    string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "The provided string argument '{0}' cannot be empty", argumentName));
        }
    }
}