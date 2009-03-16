using System;

using NHaml.Utils;

namespace NHaml.Compilers.Boo
{
    internal sealed class BooTemplateClassBuilder : TemplateClassBuilder
    {
        private bool _disableOutputIndentationShrink;

        public BooTemplateClassBuilder( string className, Type baseType )
            : base( className )
        {
            Preamble.AppendLine( Utility.FormatInvariant( "class {0}({1}):",
              ClassName, Utility.MakeBaseClassName( baseType, "[of ", "]", "." ) ) );
            Preamble.AppendLine( "  override def CoreRender(textWriter as System.IO.TextWriter):" );
        }

        public string IndentString
        {
            get { return new string( ' ', 4 + ((Depth < 0 ? 0 : Depth) * 2) ); }
        }

        public override void AppendOutput( string value, bool newLine )
        {
            if( value == null )
            {
                return;
            }

            var method = newLine ? "WriteLine" : "Write";

            if( !_disableOutputIndentationShrink && Depth > 0 )
            {
                if( value.StartsWith( string.Empty.PadLeft( Depth * 2 ), StringComparison.Ordinal ) )
                {
                    value = value.Remove( 0, Depth * 2 );
                }
            }

            // prevents problems with " at the end of the string
            value = value.Replace( "\"", "\"\"\"+'\"'+\"\"\"" );

            Output.AppendLine( Utility.FormatInvariant( IndentString + "textWriter.{0}(\"\"\"{1}\"\"\")", method, value ) );
        }

        public override void AppendCode( string code, bool newLine, bool escapeHtml )
        {
            if( code != null )
            {
                code = "(Convert.ToString(" + code + "))";

                if( escapeHtml )
                {
                    code = "(HttpUtility.HtmlEncode" + code + ")";
                }

                Output.AppendLine( IndentString + "textWriter." + (newLine ? "WriteLine" : "Write") + code + ";" );
            }
        }

        public override void AppendChangeOutputDepth( int depth )
        {
            AppendChangeOutputDepth( depth, false );
        }

        public void AppendChangeOutputDepth( int depth,
          bool disableOutputIndentationShrink )
        {
            _disableOutputIndentationShrink = disableOutputIndentationShrink;

            if( BlockDepth != depth )
            {
                Output.AppendLine( IndentString + "Output.Depth = " + depth );
                BlockDepth = depth;
            }
        }

        public override void AppendSilentCode( string code, bool closeStatement )
        {
            if( code == null )
            {
                return;
            }

            Output.Append( IndentString + code.Trim() );

            if( !closeStatement && !code.EndsWith( ":", StringComparison.OrdinalIgnoreCase ) )
            {
                Output.Append( ":" );
            }

            Output.AppendLine();
        }

        public override void AppendAttributeCode( string name, string code )
        {
            var varName = "a" + AttributeCount++;

            Output.AppendLine( IndentString + varName + "=Convert.ToString(" + code + ")" );
            Output.AppendLine( IndentString + "unless " + varName + " == null:" );
            BeginCodeBlock();
            AppendOutput( name + "=\"" );
            AppendSilentCode( "textWriter.Write(" + varName + ")", true );
            AppendOutput( "\"" );
            EndCodeBlock();
        }

        public override void BeginCodeBlock()
        {
            Depth++;
        }

        public override void EndCodeBlock()
        {
            Depth--;
        }

        public override void AppendPreambleCode( string code )
        {
            Preamble.AppendLine( new string( ' ', 4 ) + (code).Trim() );
        }

        public override string Build()
        {
            Preamble.Append( Output );

            return Preamble.ToString();
        }
    }
}