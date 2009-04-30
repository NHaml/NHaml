using System;
using System.Collections.Generic;
using System.Text;
using NHaml.Utils;

namespace NHaml.Compilers.IronRuby
{
    internal sealed class IronRubyTemplateClassBuilder : TemplateClassBuilder
    {
        public IronRubyTemplateClassBuilder( string className, Type templateBaseType )
            : base( className, templateBaseType )
        {
        }

        public override void AppendAttribute(string schema, string name, List<AttributeValueParser.Item> values)
        {
            var code = new StringBuilder();
            foreach (var item in values)
            {
                if (item.IsCode)
                {
                    code.AppendFormat("{0} + ", item.Value);
                }
                else
                {
                    code.AppendFormat("\"{0}\" + ", item.Value.Replace("\"", "\\\""));
                }
            }
            code.Remove(code.Length - 2, 2);
            var format = string.Format("RenderAttributeIfValueNotNull(text_writer, \"{0}\", \"{1}\", {2})", schema, name, code);
            AppendSilentCode(format, true);
        }

        public override void AppendPreambleCode( string code )
        {
            Preamble.AppendLine( code );
        }

        public override void AppendOutput( string value, bool newLine )
        {
            if( value == null )
            {
                return;
            }

            var method = newLine ? "WriteLine" : "Write";

            if( Depth > 0 )
            {
                if( value.StartsWith( string.Empty.PadLeft( Depth * 2 ), StringComparison.Ordinal ) )
                {
                    value = value.Remove( 0, Depth * 2 );
                }
            }

            Output.AppendLine( "text_writer." + method + "('" + value.Replace( "'", "\\'" ) + "')" );
        }

        public override void AppendCode( string code, bool newLine, bool escapeHtml )
        {
            if( code != null )
            {
                if( escapeHtml )
                {
                    code = "System::Web::HttpUtility.HtmlEncode(" + code + ")";
                }

                Output.AppendLine( "text_writer." + (newLine ? "WriteLine" : "Write") + "(" + code + ")" );
            }
        }

        public override void AppendChangeOutputDepth( int depth )
        {
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

        public override void EndCodeBlock()
        {
            Output.AppendLine( "end" );

            base.EndCodeBlock();
        }

        public override string Build()
        {
            Output.AppendLine( "end;end" );

            var result = new StringBuilder();

            result.AppendLine( "class " + ClassName + "<" + Utility.MakeBaseClassName( BaseType, "[", "]", "::" ) );

            result.AppendLine( "def __a(as)" );
            result.AppendLine( "as.collect { |k,v| " );
            result.AppendLine( "next unless v" );
            result.AppendLine( "\"#{k.to_s.gsub('_','-')}=\\\"#{v}\\\"\"" );
            result.AppendLine( "}.compact.join(' ')" );
            result.AppendLine( "end" );

            result.AppendLine( "def core_render(text_writer)" );

            result.Append(Preamble);

            result.Append(Output);

            return result.ToString();
        }
    }
}