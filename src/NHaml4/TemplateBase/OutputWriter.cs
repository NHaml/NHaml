using System;
using System.IO;

namespace NHaml4.TemplateBase
{
    public class OutputWriter
    {
        private bool _indentComing = true;

        public TextWriter TextWriter { get; set; }

        public void WriteLine( string value )
        {
            WriteIndent();

            TextWriter.WriteLine( value );

            _indentComing = true;
        }

        public void Write( string value )
        {
            WriteIndent();

            TextWriter.Write( value );
        }

        public void Indent()
        {
            Depth++;
        }

        public void Outdent()
        {
            Depth = Math.Max( 0, Depth - 1 );
        }

        public int Depth { get; set; }

        private void WriteIndent()
        {
            if( _indentComing )
            {
                TextWriter.Write( string.Empty.PadLeft( Depth * 2 ) );

                _indentComing = false;
            }
        }
    }
}