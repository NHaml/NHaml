using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

using NHaml.Utils;

namespace NHaml.Exceptions
{
    [Serializable]
    [DebuggerStepThrough]
    public class TemplateCompilationException : Exception
    {
        public static void Throw( CompilerResults compilerResults, string templateSource, string templatePath )
        {
            var message = new StringBuilder();
            var lines = templateSource.Replace( "\r", "" ).Split( '\n' );

            message.AppendLine(Utility.FormatCurrentCulture("An error occurred when compiling the file: {0}", templatePath));
            message.AppendLine();

            foreach( CompilerError error in compilerResults.Errors )
            {
                message.AppendLine( error.ToString() );

                if( error.Line > 0 )
                {
                    var line = error.Line - 1;

                    if( line - 1 > 0 )
                    {
                        message.AppendLine( string.Format("{0}: {1}", (line - 1).ToString( "0000", CultureInfo.CurrentUICulture ), lines[line - 1]) );
                    }

                    message.AppendLine( string.Format("{0}: {1}", (line - 1).ToString( "0000", CultureInfo.CurrentUICulture ), lines[line]) );

                    if( line + 1 < lines.Length )
                    {
                        message.AppendLine( string.Format("{0}: {1}", (line + 1).ToString( "0000", CultureInfo.CurrentUICulture ), lines[line + 1]) );
                    }

                    message.AppendLine();
                }
            }
            throw new TemplateCompilationException( message.ToString(), compilerResults, templateSource );
        }

        private TemplateCompilationException( string message, CompilerResults compilerResults, string compiledTemplateSource )
            : base( message )
        {
            CompilerResults = compilerResults;
            TemplateSource = compiledTemplateSource;
        }


        public TemplateCompilationException( string message )
            : base( message )
        {
        }

        public TemplateCompilationException( string message, Exception innerException )
            : base( message, innerException )
        {
        }

        private TemplateCompilationException( SerializationInfo info, StreamingContext context )
            : base( info, context )
        {
        }

        public CompilerResults CompilerResults { get; private set; }

        public string TemplateSource { get; private set; }

        [SecurityPermission( SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter )]
        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );

            info.AddValue( "_compilerResults", CompilerResults );
            info.AddValue( "_templateSource", TemplateSource );
        }
    }
}