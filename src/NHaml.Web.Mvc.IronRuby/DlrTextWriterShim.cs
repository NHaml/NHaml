using System.IO;
using System.Text;

namespace NHaml.Web.Mvc.IronRuby
{
    /// <summary>
    /// because IronRuby CLS method resolution is still flakey.
    /// </summary>
    internal class DlrTextWriterShim : TextWriter
    {
        private readonly TextWriter _textWriter;

        public DlrTextWriterShim( TextWriter textWriter )
            : base( textWriter.FormatProvider )
        {
            _textWriter = textWriter;
        }

        public override Encoding Encoding
        {
            get { return _textWriter.Encoding; }
        }

        public override void Write( char value )
        {
            _textWriter.Write( value );
        }

        public override void Write( char[] buffer, int index, int count )
        {
            _textWriter.Write( buffer, index, count );
        }

        public override void Write( string value )
        {
            _textWriter.Write( value );
        }
    }
}