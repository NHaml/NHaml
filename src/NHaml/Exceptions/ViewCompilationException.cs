using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

using NHaml.Properties;
using NHaml.Utils;

namespace NHaml.Exceptions
{
    [Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    public sealed class ViewCompilationException : Exception
    {
        private readonly CompilerResults _compilerResults;
        private readonly string _viewSource;

        public static void Throw( CompilerResults compilerResults, string viewSource, string templatePath )
        {
            var message = new StringBuilder();
            var lines = viewSource.Replace( "\r", "" ).Split( '\n' );

            message.AppendLine( Utility.FormatCurrentCulture( Resources.CompilationError, templatePath ) );
            message.AppendLine();

            foreach( CompilerError error in compilerResults.Errors )
            {
                message.AppendLine( error.ToString() );

                if( error.Line > 0 )
                {
                    var line = error.Line - 1;

                    if( line - 1 > 0 )
                        message.AppendLine( (line - 1).ToString( "0000" ) + ": " + lines[line - 1] );

                    message.AppendLine( (line - 1).ToString( "0000" ) + ": " + lines[line] );

                    if( line + 1 < lines.Length )
                        message.AppendLine( (line + 1).ToString( "0000" ) + ": " + lines[line + 1] );

                    message.AppendLine();
                }
            }

            throw new ViewCompilationException( message.ToString(), compilerResults, viewSource );
        }

        private ViewCompilationException( string message, CompilerResults compilerResults, string compiledViewSource )
            : base( message )
        {
            _compilerResults = compilerResults;
            _viewSource = compiledViewSource;
        }

        public ViewCompilationException()
        {
        }

        public ViewCompilationException( string message )
            : base( message )
        {
        }

        public ViewCompilationException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        private ViewCompilationException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public CompilerResults CompilerResults
        {
            get { return _compilerResults; }
        }

        public string ViewSource
        {
            get { return _viewSource; }
        }

        [SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );

            info.AddValue( "_compilerResults", _compilerResults );
            info.AddValue( "_viewSource", _viewSource );
        }
    }
}