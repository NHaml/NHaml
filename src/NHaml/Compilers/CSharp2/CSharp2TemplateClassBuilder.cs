using System;

using NHaml.Utils;

namespace NHaml.Compilers.CSharp2
{
    internal sealed class CSharp2TemplateClassBuilder : TemplateClassBuilder
    {
        public CSharp2TemplateClassBuilder( string className, Type baseType )
            : base( className )
        {
            Preamble.AppendLine( Utility.FormatInvariant( "public class {0} : {1} {{",
              ClassName, Utility.MakeBaseClassName( baseType, "<", ">", "." ) ) );
            Preamble.AppendLine( "protected override void CoreRender(TextWriter textWriter){" );
        }

        public override void AppendOutput( string value, bool newLine )
        {
            if( value == null )
            {
                return;
            }

            var method = newLine ? "WriteLine" : "Write";

            value = value.Replace( "\"", "\"\"" );

            if( Depth > 0 )
            {
                if( value.StartsWith( string.Empty.PadLeft( Depth * 2 ), StringComparison.Ordinal ) )
                {
                    value = value.Remove( 0, Depth * 2 );
                }
            }

            Output.AppendLine( "textWriter." + method + "(@\"" + value + "\");" );
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

                Output.AppendLine( "textWriter." + (newLine ? "WriteLine" : "Write") + code + ";" );
            }
        }

        public override void AppendChangeOutputDepth( int depth )
        {
            if( BlockDepth != depth )
            {
                Output.AppendLine( "Output.Depth = " + depth + ";" );

                BlockDepth = depth;
            }
        }

        public override void AppendSilentCode( string code, bool closeStatement )
        {
            if( code != null )
            {
                code = code.Trim();

                if( closeStatement && !code.EndsWith( ";", StringComparison.Ordinal ) )
                {
                    code += ';';
                }

                Output.AppendLine( code );
            }
        }


        public override void AppendAttributeCode( string name, string code )
        {
            var format = string.Format("RenderAttributeIfValueNotNull(textWriter, \"{0}\",Convert.ToString({1}));", name, code);
            AppendSilentCode(format, false);
        }

        public override void BeginCodeBlock()
        {
            Depth++;
            Output.AppendLine( "{" );
        }

        public override void EndCodeBlock()
        {
            Output.AppendLine( "}" );
            Depth--;
        }

        public override void AppendPreambleCode( string code )
        {
            Preamble.AppendLine( code + ';' );
        }

        public override string Build()
        {
            Output.Append( "}}" );

            Preamble.Append( Output );

            return Preamble.ToString();
        }
    }
}