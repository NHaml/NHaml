using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

using NHaml.Utils;

namespace NHaml.Exceptions
{
    [Serializable]
    [DebuggerStepThrough]
    public  class SyntaxException : Exception
    {
        private readonly InputLine _inputLine;

        public SyntaxException()
        {
        }

        public SyntaxException( string message )
            : base( message )
        {
        }

        public SyntaxException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        private SyntaxException( string message, InputLine inputLine )
            : base( message )
        {
            _inputLine = inputLine;
        }

        private SyntaxException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public InputLine InputLine
        {
            get { return _inputLine; }
        }

        public static void Throw( InputLine inputLine, string errorFormat, params object[] values )
        {
            var message = Utility.FormatCurrentCulture( "Syntax error on line {0}: {1}: '{2}'", inputLine.LineNumber,
              Utility.FormatCurrentCulture( errorFormat, values ),
              inputLine.Text );

            throw new SyntaxException( message, inputLine );
        }

        [SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );

            info.AddValue( "_line", _inputLine );
        }
    }
}